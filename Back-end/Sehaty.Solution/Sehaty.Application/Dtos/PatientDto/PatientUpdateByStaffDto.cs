namespace Sehaty.Application.Dtos.PatientDto
{
    public class PatientUpdateByStaffDto
    {
        [Required]
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChrinicConditions { get; set; }
        [EnumDataType(typeof(PatientStatus))]
        public PatientStatus Status { get; set; }

    }
}
