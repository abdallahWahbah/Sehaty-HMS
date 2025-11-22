using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Sehaty.Application.Services.Contract.BusinessServices.Contract;
using Sehaty.Core.Entities.Business_Entities;

namespace Sehaty.Application.Services
{
    public class PrescriptionPdfService : IPrescriptionPdfService
    {
        public PrescriptionPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
        public byte[] GeneratePrescriptionPdf(Prescription prescription)
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
                            row.RelativeItem().Text($"MRN: {prescription.Patient.MRN}").FontSize(12);
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

                            foreach (var med in prescription.Medications)
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
