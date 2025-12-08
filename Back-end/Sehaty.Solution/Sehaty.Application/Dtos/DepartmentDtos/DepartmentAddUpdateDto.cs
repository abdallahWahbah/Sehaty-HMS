namespace Sehaty.Application.Dtos.DepartmentDtos
{
    public class DepartmentAddUpdateDto
    {
        [Required, MaxLength(100)]
        public string En_Name { get; set; }

        [Required, MaxLength(100)]
        public string Ar_Name { get; set; }
        public string Description { get; set; }
    }
}
