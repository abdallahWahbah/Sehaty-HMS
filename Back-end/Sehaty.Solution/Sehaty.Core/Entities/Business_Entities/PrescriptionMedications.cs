namespace Sehaty.Core.Entities.Business_Entities
{
    public class PrescriptionMedications : BaseEntity
    {
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }
        public int MedicationId { get; set; }
        public Medication Medication { get; set; }
    }
}
