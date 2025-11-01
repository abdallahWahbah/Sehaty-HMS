using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entites
{
    public class DoctorAvailabilitySlot : BaseEntity
    {
        [ForeignKey(nameof(Doctors))]
        [Column(TypeName = "nvarchar(10)")]
        public string Doctor_ID { get; set; }
        public Doctor Doctors { get; set; }
        [Column(TypeName = "nvarchar(10)")]

        public string? Day_of_Week { get; set; }
        public TimeOnly Start_Time { get; set; }
        public TimeOnly End_Time { get; set; }
        public bool Is_Recurring { get; set; }
        public DateOnly? Date { get; set; }
        public bool Available_Flag { get; set; }
    }
}
