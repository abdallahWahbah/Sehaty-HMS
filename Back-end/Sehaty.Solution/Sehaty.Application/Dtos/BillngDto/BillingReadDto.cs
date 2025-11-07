using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingReadDto
    {
        public string Name { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public BillingStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string ItemsDetail { get; set; }
        public string TransactionId { get; set; }
        public decimal? CommissionApplied { get; set; }
        public decimal? NetAmount { get; set; }
        public string Notes { get; set; }
    }
}
