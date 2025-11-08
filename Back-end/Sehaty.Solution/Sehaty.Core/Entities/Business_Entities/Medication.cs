using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.Business_Entities
{
    public class Medication : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<PrescriptionMedications> Prescriptions { get; set; } = new List<PrescriptionMedications>();

    }
}
