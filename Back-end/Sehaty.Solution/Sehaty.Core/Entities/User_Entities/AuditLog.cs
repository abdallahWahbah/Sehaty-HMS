using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.User_Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Action { get; set; } = null!;

        public string? IpAdress { get; set; }

        public DateTime CreatAt { get; set; } = DateTime.UtcNow;
    }
}
