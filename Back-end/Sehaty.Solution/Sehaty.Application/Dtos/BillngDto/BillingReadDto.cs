namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingReadDto
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public BillingStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
