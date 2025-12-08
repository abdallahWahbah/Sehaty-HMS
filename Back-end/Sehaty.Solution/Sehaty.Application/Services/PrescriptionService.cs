namespace Sehaty.Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal User;

        public PrescriptionService(IUnitOfWork unit,IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
            this.unit = unit;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            User = httpContextAccessor.HttpContext?.User;
            QuestPDF.Settings.License = LicenseType.Community;
        }
        public async Task<Result<Prescription>> CreatePrescriptionAsync(CreatePrescriptionsDto dto)
        {
            bool isAppointmentAlreadyHasPrescription = await unit.Repository<Prescription>()
                            .AnyAsync(P => P.AppointmentId == dto.AppointmentId);

            if(isAppointmentAlreadyHasPrescription)
                return Result<Prescription>.Failure(ErrorType.BadRequest,"This Appointment Already Has Its Prescription You Can Edit It If You Need.");


            var appointment = await unit.Repository<Appointment>().GetByIdAsync(dto.AppointmentId);

            if(appointment is null)
                return Result<Prescription>.Failure(ErrorType.NotFound,"Appointment not found");
            if(appointment?.Status != AppointmentStatus.InProgress && appointment?.Status != AppointmentStatus.Completed)
                return Result<Prescription>.Failure(ErrorType.BadRequest,
                       "Oops! You can add a prescription only when the appointment is In Progress or Completed.");


            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var doctor = await unit.Repository<Doctor>().GetFirstOrDefaultAsync(D => D.UserId == doctorUserId);

            if(doctor is null)
                return Result<Prescription>.Failure(ErrorType.NotFound,"Doctor not found");


            var medicalRecord = await unit.Repository<MedicalRecord>()
                .GetFirstOrDefaultAsync(M => M.PatientId == dto.PatientId);

            if(medicalRecord is null)
            {
                var medicalRecordDto = new MedicalRecordAddByDoctorDto
                {
                    PatientId = dto.PatientId,
                    Symptoms = "To be updated",
                    Diagnosis = "Initial diagnosis pending",
                    TreatmentPlan = "To be determined",
                    RecordType = RecordType.Diagnosis
                };

                medicalRecord = mapper.Map<MedicalRecord>(medicalRecordDto);
                await unit.Repository<MedicalRecord>().AddAsync(medicalRecord);
                await unit.CommitAsync();
            }

            var prescription = mapper.Map<Prescription>(dto);

            prescription.DoctorId = doctor.Id;
            prescription.MedicalRecordId = medicalRecord.Id;

            appointment.Status = AppointmentStatus.Completed;

            await unit.Repository<Prescription>().AddAsync(prescription);
            unit.Repository<Appointment>().Update(appointment);

            await unit.CommitAsync();
            return Result<Prescription>.Success(prescription);
        }

        public async Task<Result<IEnumerable<Prescription>>> GetPatientPrescriptionsAsync(int patientId)
        {
            var spec = new PrescriptionSpecifications(P => P.PatientId == patientId);
            var prescriptions = await unit.Repository<Prescription>().GetAllWithSpecAsync(spec);
            if(prescriptions == null)
                return Result<IEnumerable<Prescription>>.Failure(ErrorType.NotFound,"Patient Has No Prescription Yet");
            var sortedprescriptions = prescriptions
                    .OrderByDescending(p => p.DateIssued)
                    .ToList();
            return Result<IEnumerable<Prescription>>.Success(sortedprescriptions);
        }

        public async Task<Result<Prescription>> GetPrescriptionDetailsAsync(int id)
        {
            var doctorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var doctorId = unit.Repository<Doctor>().FindBy(D => D.UserId == doctorUserId).Select(D => D.Id).FirstOrDefault();
            var spec = new PrescriptionSpecifications(P => P.Id == id && P.DoctorId == doctorId);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if(prescription == null)
                return Result<Prescription>.Failure(ErrorType.NotFound,"Prescription Not Found");
            return Result<Prescription>.Success(prescription);
            ;
        }

        public async Task<Result> UpdatePrescriptionAsync(int id,UpdatePrescriptionDto dto)
        {
            var spec = new PrescriptionSpecifications(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if(prescription == null)
                return Result.Failure(ErrorType.NotFound,"Prescription Not Found");
            mapper.Map(dto,prescription);
            unit.Repository<Prescription>().Update(prescription);
            await unit.CommitAsync();
            return Result.Success();

        }

        public async Task<Result> DeletePrescriptionAsync(int id)
        {
            var prescription = await unit.Repository<Prescription>().GetByIdAsync(id);
            if(prescription == null)
                return Result.Failure(ErrorType.NotFound,"Prescription Not Found");
            unit.Repository<Prescription>().Delete(prescription);
            await unit.CommitAsync();
            return Result.Success();
        }

        public async Task<Result<byte[]>> GetPrescriptionPdfFile(int id)
        {
            PrescriptionSpecifications spec = new(id);
            var prescription = await unit.Repository<Prescription>().GetByIdWithSpecAsync(spec);
            if(prescription is null)
                return Result<byte[]>.Failure(ErrorType.NotFound,"Prescription Not Found");

            return Result<byte[]>.Success(GeneratePrescriptionPdf(prescription));
        }
        private static byte[] GeneratePrescriptionPdf(Prescription prescription)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Header().PaddingBottom(10).AlignCenter().Text("Sehaty Hospital - Prescription")
                                 .FontSize(20).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingBottom(5).Row(row =>
                        {
                            row.RelativeItem().Text($"Patient: {prescription.Patient.FirstName} {prescription.Patient.LastName}").FontSize(12);
                            //row.RelativeItem().Text($"MRN: {prescription.Patient.MRN}").FontSize(12);
                        });
                        col.Item().PaddingBottom(5).Row(row =>
                        {
                            row.RelativeItem().Text($"Doctor: {prescription.Doctor.FirstName} {prescription.Doctor.LastName}").FontSize(12);
                            row.RelativeItem().Text($"Date: {prescription.DateIssued:yyyy-MM-dd}").FontSize(12);
                        });
                        col.Item().PaddingBottom(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Status: {prescription.Status}").FontSize(12);
                            row.RelativeItem().Text($"License number: {prescription.Doctor.LicenseNumber}").FontSize(12);
                        });

                        col.Item().PaddingBottom(5).Text("Medications:").FontSize(14).Bold().Underline();

                        col.Item().PaddingBottom(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Padding(5).Text("Medication Name").Bold().FontSize(12);
                                header.Cell().Padding(5).Text("Dosage").Bold().FontSize(12);
                                header.Cell().Padding(5).Text("Frequency").Bold().FontSize(12);
                                header.Cell().Padding(5).Text("Duration").Bold().FontSize(12);
                            });

                            foreach(var med in prescription.Medications)
                            {
                                table.Cell().Padding(5).Text(med.Medication.Name).FontSize(11);
                                table.Cell().Padding(5).Text(med.Dosage).FontSize(11);
                                table.Cell().Padding(5).Text(med.Frequency).FontSize(11);
                                table.Cell().Padding(5).Text(med.Duration).FontSize(11);
                            }
                        });

                        col.Item().PaddingTop(10).Text($"Doctor’s Digital Signature: {prescription.DigitalSignature}").FontSize(12);
                    });
                    page.Footer().PaddingTop(5).AlignCenter().Text("© 2025 Sehaty Hospital").FontSize(10);
                });
            });

            return document.GeneratePdf();
        }

    }
}
