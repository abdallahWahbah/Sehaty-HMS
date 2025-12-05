namespace Sehaty.Application.Dtos.AppointmentDTOs
{
    public class ConfirmAppointmentResponseDto
    {
        public bool Success { get; set; }
        public string Payment_link { get; set; }
        public int TotalAmount { get; set; }
        public int Order_id { get; set; }
        public string BillingId { get; set; }
    }

}
