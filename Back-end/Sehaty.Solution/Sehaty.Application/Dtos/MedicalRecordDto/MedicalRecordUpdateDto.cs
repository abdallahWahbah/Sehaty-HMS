namespace Sehaty.Application.Dtos.MedicalRecordDto
{
    public class MedicalRecordUpdateDto
    {
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public int? BpSystolic { get; set; }
        public int? BpDiastolic { get; set; }
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Weight { get; set; }
        public string VitalBp { get; set; }
        public string Notes { get; set; }
        public RecordType? RecordType { get; set; }
        public bool? IsFinialize { get; set; }
    }
}
