namespace Sehaty.Core.Entities.Business_Entities
{
    public class Medication : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PrescriptionMedications> Prescriptions { get; set; } = new List<PrescriptionMedications>();

    }
}
