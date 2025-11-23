namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IPrescriptionPdfService
    {
        byte[] GeneratePrescriptionPdf(Prescription prescription);
    }
}
