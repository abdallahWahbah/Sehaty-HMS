using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.MedicationDTOs
{
    public class MedicationDto
    {
        [Required]
        [MaxLength(100)]
        public string MedicationName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dosage { get; set; }

        [Required]
        [MaxLength(100)]
        public string Frequency { get; set; }

        [Required]
        [MaxLength(100)]
        public string Duration { get; set; }
    }
}
