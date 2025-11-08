using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Core.Entities.User_Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? CreatedByIp { get; set; }
        public int UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
