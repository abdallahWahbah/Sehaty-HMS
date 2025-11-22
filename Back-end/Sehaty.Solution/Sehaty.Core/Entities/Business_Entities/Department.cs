namespace Sehaty.Core.Entites
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string NameLocal { get; set; }
        public string Description { get; set; }
        public List<Doctor> Doctors { get; set; }
    }
}
