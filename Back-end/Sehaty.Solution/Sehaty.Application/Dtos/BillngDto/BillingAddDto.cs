namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingAddDto
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ItemsDetail { get; set; }
        public string Notes { get; set; }

    }
}
