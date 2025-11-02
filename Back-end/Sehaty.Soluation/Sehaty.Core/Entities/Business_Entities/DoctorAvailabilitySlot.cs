using System.ComponentModel.DataAnnotations.Schema;

namespace Sehaty.Core.Entites
{
    public class DoctorAvailabilitySlot : BaseEntity
    {
        [ForeignKey(nameof(Doctor))]
        [Column(TypeName = "int")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public string DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly Date { get; set; }
        public bool AvailableFlag { get; set; }
    }
}
