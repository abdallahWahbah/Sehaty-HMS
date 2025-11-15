using Sehaty.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.Business_Entities.MedicalRecords
{
    public class MedicalRecordAuditLog : BaseEntity
    {

        public int MedicalRecordId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int UpdatedByDoctorId { get; set; }
    }
}
