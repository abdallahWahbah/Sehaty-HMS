namespace Sehaty.Infrastructure.Dtos
{
    public class MedicalRecordAddOrUpdateByNurseDto
    {
        public int AppointmentId { get; set; }
        public int? BpSystolic { get; set; }
        public int? BpDiastolic { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
    }
}
