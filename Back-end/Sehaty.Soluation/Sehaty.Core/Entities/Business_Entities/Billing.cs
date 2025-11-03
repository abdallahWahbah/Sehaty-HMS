using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entites
{
    public class Billing : BaseEntity
    {
        public int PatientId { get; set; }           
        public int AppointmentId { get; set; }       
        public DateTime BillingDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string InvoiceNumber { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
    }
}
