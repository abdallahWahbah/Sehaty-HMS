using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class ChangeUserRoleDto
    {
        public int UserId { get; set; }
        public int NewRoleId { get; set; }
    }
}
