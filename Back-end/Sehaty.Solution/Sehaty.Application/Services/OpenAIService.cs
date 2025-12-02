namespace Sehaty.Application.Services
{
    public class OpenAIService(IUnitOfWork unitOfWork,IHttpClientFactory httpClientFactory,IConfiguration config,IMapper mapper) : IOpenAIService
    {

        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly string _openAiApiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("ApiKey");

        public async Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId)
        {
            var spec = new PrescriptionSpecifications(prescriptionId);
            var prescription = await unitOfWork.Repository<Prescription>().GetByIdWithSpecAsync(spec);

            if(prescription == null)
                return Result<PrescriptionAnalysisResponseDto>.Failure(ErrorType.NotFound,"Prescription not found");

            string prompt = BuildAnalysisPrompt(prescription);

            var result = await CallOpenAiAsync(prompt);
            if(!result.IsSuccess)
                return Result<PrescriptionAnalysisResponseDto>.Failure(result.ErrorType,result.Error);

            string aiResponse = result.Data;
            var analysisResponse = mapper.Map<PrescriptionAnalysisResponseDto>(prescription);
            analysisResponse.AnalysisResult = aiResponse;
            analysisResponse.MedicationsAnalysis = ParseMedicationsFromAI(prescription);

            //var analysisResponse = new PrescriptionAnalysisResponseDto
            //{
            //    PrescriptionId = prescription.Id,
            //    PatientName = prescription.Patient?.FirstName + " " + prescription.Patient?.LastName ?? "غير متوفر",
            //    DoctorName = prescription.Doctor?.FirstName + " " + prescription.Doctor?.LastName ?? "غير متوفر",
            //    DateIssued = prescription.DateIssued,
            //    AnalysisResult = aiResponse,
            //    MedicationsAnalysis = ParseMedicationsFromAI(aiResponse, prescription),
            //    GeneralInstructions = prescription.SpecialInstructions ?? "لا توجد تعليمات خاصة"
            //};

            return Result<PrescriptionAnalysisResponseDto>.Success(analysisResponse);
        }


        public async Task<Result<PatientHistoryAnalysisResponseDto>> AnalyzePatientHistoryAsync(int patientId)
        {
            var spec = new MedicalRecordSpec(m => m.PatientId == patientId);
            var medicalRecords = await unitOfWork.Repository<MedicalRecord>().GetAllWithSpecAsync(spec);

            if(!medicalRecords.Any())
                return Result<PatientHistoryAnalysisResponseDto>.Failure(
                    ErrorType.NotFound,
                    "No medical records found for this patient");

            var specPatient = new PatientSpecifications(mr => mr.Id == patientId);
            var patient = await unitOfWork.Repository<Patient>().GetByIdWithSpecAsync(specPatient);

            if(patient == null)
                return Result<PatientHistoryAnalysisResponseDto>
                    .Failure(ErrorType.NotFound,"Patient not found");

            var specprescriptions = new PrescriptionSpecifications(mr => mr.PatientId == patientId);

            var prescriptions = await unitOfWork.Repository<Prescription>().GetAllWithSpecAsync(specprescriptions);

            string prompt = BuildPatientHistoryPrompt(patient,medicalRecords,prescriptions);

            var aiResult = await CallOpenAiAsync(prompt);
            if(!aiResult.IsSuccess)
                return Result<PatientHistoryAnalysisResponseDto>.Failure(
                    aiResult.ErrorType,
                    aiResult.Error);

            #region Response
            var response = new PatientHistoryAnalysisResponseDto
            {
                PatientId = patientId,
                PatientName = $"{patient.FirstName} {patient.LastName}",
                TotalPrescriptions = prescriptions.Count(),
                AISummary = aiResult.Data,
                Records = [.. medicalRecords.Select(r => new RecordSummaryDto
                {
                    RecordId = r.Id,
                    RecordDate = r.RecordDate,
                    RecordType = r.RecordType.ToString(),
                    Diagnosis = r.Diagnosis ?? "N/A",
                    Symptoms = r.Symptoms ?? "N/A",
                    TreatmentPlan = r.TreatmentPlan ?? "N/A",
                    Medications = [.. prescriptions
                        .Where(p => p.MedicalRecordId == r.Id)
                        .SelectMany(p => p.Medications)
                        .Select(m => m.Medication?.Name ?? "Unknown")
                        .Distinct()]
                })]
                #endregion
            };

            return Result<PatientHistoryAnalysisResponseDto>.Success(response);
        }


        public async Task<Result<SymptomsAnalysisResponseDto>> AnalyzeSymptomsAndSuggestAppointmentAsync(SymptomsAnalysisRequestDto request)
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(request.PatientId);
            if(patient == null)
                return Result<SymptomsAnalysisResponseDto>.Failure(ErrorType.NotFound,"Patient not found");

            string prompt = BuildSymptomsAnalysisPrompt(request.Symptoms);

            var aiResult = await CallOpenAiAsync(prompt);
            if(!aiResult.IsSuccess)
                return Result<SymptomsAnalysisResponseDto>.Failure(aiResult.ErrorType,aiResult.Error);

            string suggestedSpecialization = aiResult.Data;

            var doctorSpec = new DoctorSpecifications(d =>
                d.Department.Name.Contains(suggestedSpecialization,StringComparison.OrdinalIgnoreCase));

            var doctors = await unitOfWork.Repository<Doctor>().GetAllWithSpecAsync(doctorSpec);

            if(!doctors.Any())
                return Result<SymptomsAnalysisResponseDto>.Failure(
                    ErrorType.NotFound,
                    $"No doctors found for specialization: {suggestedSpecialization}");

            var allAvailableSlots = new List<SuggestedSlotDto>();
            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            foreach(var doctor in doctors)
            {
                var slotSpec = new DoctorAppointmentSlotspec(s => s.DoctorId == doctor.Id && !s.IsBooked && s.Date >= today);

                var slots = await unitOfWork.Repository<DoctorAppointmentSlot>()
                    .GetAllWithSpecAsync(slotSpec);

                foreach(var slot in slots)
                {
                    if(slot.Date == today && slot.StartTime <= currentTime)
                        continue;

                    allAvailableSlots.Add(new SuggestedSlotDto
                    {
                        SlotId = slot.Id,
                        DoctorId = doctor.Id,
                        DoctorName = $"{doctor.FirstName} {doctor.LastName}",
                        Specialization = doctor.Specialty,
                        Date = slot.Date,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        ConsultationFee = doctor.DetectionPrice
                    });
                }
            }

            if(!(allAvailableSlots.Count == 0))
                return Result<SymptomsAnalysisResponseDto>.Failure(
                    ErrorType.NotFound,
                    "No available appointments found for this specialization");

            var nearestSlot = allAvailableSlots
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .FirstOrDefault();

            var response = new SymptomsAnalysisResponseDto
            {
                AnalyzedSymptoms = $"تم تحليل الأعراض: {request.Symptoms}",
                SuggestedSpecialization = suggestedSpecialization,
                AvailableSlots = nearestSlot != null
                    ? [nearestSlot]
                    : []
            };

            return Result<SymptomsAnalysisResponseDto>.Success(response);
        }




        #region Helper Function

        private static string BuildSymptomsAnalysisPrompt(string symptoms)
        {
            var sb = new StringBuilder();
            sb.AppendLine("أنت مساعد طبي ذكي متخصص في تحليل الأعراض واقتراح التخصص الطبي المناسب.");
            sb.AppendLine($"\nالأعراض المذكورة: {symptoms}");
            sb.AppendLine("\nبناءً على هذه الأعراض، ما هو التخصص الطبي الأنسب؟");
            sb.AppendLine("الرجاء الرد بكلمة واحدة فقط تمثل التخصص بالإنجليزية (مثل: Cardiology, Dermatology, Orthopedics, Neurology, إلخ)");
            sb.AppendLine("إذا لم تكن متأكداً، اختر 'General'");

            return sb.ToString();
        }
        private static string BuildPatientHistoryPrompt(Patient patient,IEnumerable<MedicalRecord> records,IEnumerable<Prescription> prescriptions)
        {
            var sb = new StringBuilder();

            sb.AppendLine("أنت مساعد طبي ذكي. قم بتحليل التاريخ المرضي الكامل للمريض التالي وقدم ملخصاً طبياً شاملاً للدكتور.");
            sb.AppendLine($"\n--- بيانات المريض ---");
            sb.AppendLine($"الاسم: {patient.FirstName} {patient.LastName}");
            sb.AppendLine($"العمر: {CalculateAge(patient.DateOfBirth)} سنة");
            sb.AppendLine($"الجنس: {patient.Gender}");
            sb.AppendLine($"فصيلة الدم: {patient.BloodType ?? "غير محدد"}");

            sb.AppendLine($"\n--- السجلات الطبية ({records.Count()}) ---");

            foreach(var record in records.OrderBy(r => r.RecordDate))
            {
                sb.AppendLine($"\n📅 التاريخ: {record.RecordDate:dd/MM/yyyy}");
                sb.AppendLine($"النوع: {record.RecordType}");

                if(!string.IsNullOrEmpty(record.Symptoms))
                    sb.AppendLine($"الأعراض: {record.Symptoms}");

                if(!string.IsNullOrEmpty(record.Diagnosis))
                    sb.AppendLine($"التشخيص: {record.Diagnosis}");

                if(!string.IsNullOrEmpty(record.TreatmentPlan))
                    sb.AppendLine($"خطة العلاج: {record.TreatmentPlan}");

                if(record.BpSystolic.HasValue || record.BpDiastolic.HasValue)
                    sb.AppendLine($"ضغط الدم: {record.BpSystolic}/{record.BpDiastolic}");

                if(record.Temperature.HasValue)
                    sb.AppendLine($"الحرارة: {record.Temperature}°C");

                if(record.HeartRate.HasValue)
                    sb.AppendLine($"النبض: {record.HeartRate} bpm");

                if(record.Weight.HasValue)
                    sb.AppendLine($"الوزن: {record.Weight} kg");

                var recordPrescriptions = prescriptions.Where(p => p.MedicalRecordId == record.Id);
                if(recordPrescriptions.Any())
                {
                    sb.AppendLine("الأدوية الموصوفة:");
                    foreach(var pres in recordPrescriptions)
                    {
                        foreach(var med in pres.Medications)
                        {
                            sb.AppendLine($"  • {med.Medication?.Name ?? "Unknown"} - {med.Dosage} - {med.Frequency}");
                        }
                    }
                }

                if(!string.IsNullOrEmpty(record.Notes))
                    sb.AppendLine($"ملاحظات: {record.Notes}");
            }

            sb.AppendLine("\n\n--- المطلوب من التحليل ---");
            sb.AppendLine("1. ملخص الحالة الطبية العامة للمريض");
            sb.AppendLine("2. الأمراض أو الحالات المزمنة إن وجدت");
            sb.AppendLine("3. تطور الحالة عبر الزمن");
            sb.AppendLine("4. الأدوية المتكررة أو طويلة الأمد");
            sb.AppendLine("5. أي ملاحظات مهمة أو تحذيرات طبية");
            sb.AppendLine("6. توصيات للمتابعة أو الفحوصات المقترحة");
            sb.AppendLine("\nالرجاء تقديم التحليل باللغة العربية بأسلوب طبي احترافي.");

            return sb.ToString();
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if(dateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }
        private static string BuildAnalysisPrompt(Prescription prescription)
        {
            var sb = new StringBuilder();


            sb.AppendLine("أنت مساعد طبي ذكي. قم بتحليل الروشتة التالية وشرحها للمريض بطريقة مبسطة وواضحة.");
            sb.AppendLine($"اسم المريض: {prescription.Patient?.FirstName + "" + prescription.Patient?.LastName}");
            sb.AppendLine($"اسم الدكتور: {prescription.Doctor?.FirstName + "" + prescription.Doctor?.LastName}");
            sb.AppendLine($"تاريخ الروشتة: {prescription.DateIssued:dd/MM/yyyy}");
            sb.AppendLine("\n--- الأدوية الموصوفة ---");

            foreach(var med in prescription.Medications)
            {
                sb.AppendLine($"\n{med.Medication?.Name ?? "Unknown"}:");
                sb.AppendLine($"  - الجرعة: {med.Dosage}");
                sb.AppendLine($"  - التكرار: {med.Frequency}");
                sb.AppendLine($"  - المدة: {med.Duration}");
            }

            if(!string.IsNullOrEmpty(prescription.SpecialInstructions))
            {
                sb.AppendLine($"\nتعليمات خاصة: {prescription.SpecialInstructions}");
            }
            sb.AppendLine("\n\nمطلوب منك:");
            sb.AppendLine("1. شرح كل دواء: ليه بياخده، إزاي ياخده، امتى ياخده");
            sb.AppendLine("2. الآثار الجانبية المحتملة لكل دواء");
            sb.AppendLine("3. أي تحذيرات مهمة");
            sb.AppendLine("4. نصائح عامة للمريض");
            sb.AppendLine("\nيرجى الرد باللغة العربية بأسلوب واضح وسهل الفهم.");

            return sb.ToString();
        }

        private async Task<Result<string>> CallOpenAiAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = "أنت مساعد طبي متخصص في شرح الروشتات الطبية ومساعدة الأطباء فى بعض الحالات الطبية والتشخصيات الطبية المُحتملة." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent,Encoding.UTF8,"application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {_openAiApiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions",httpContent);

            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure(ErrorType.BadRequest,$"OpenAI API Error: {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var aiMessage = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if(aiMessage != null)
            {
                aiMessage = CleanMarkdown(aiMessage);
                return Result<string>.Success(aiMessage);
            }
            return Result<string>.Success("No response from AI");

        }

        private static List<MedicationAnalysisDto> ParseMedicationsFromAI(Prescription prescription)
        {
            var result = new List<MedicationAnalysisDto>();

            foreach(var med in prescription.Medications)
            {
                result.Add(new MedicationAnalysisDto
                {
                    MedicationName = med.Medication?.Name ?? "Unknown",
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    Duration = med.Duration
                });
            }
            return result;
        }

        private static string CleanMarkdown(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                return text;

            // Remove Markdown headings #### ### ##
            text = Regex.Replace(text,@"#{1,6}\s*","");

            // Remove bold & italics symbols **, *, __, _
            text = Regex.Replace(text,@"(\*\*|\*|__|_)","");

            // Remove extra backticks ```
            text = text.Replace("```","");

            return text.Trim();
        }

        #endregion
    }
}
