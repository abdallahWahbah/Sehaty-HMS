using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.BillngDto
{
    public class BillingAddDto
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ItemsDetail { get; set; }
        public string Notes { get; set; }

    }
}
