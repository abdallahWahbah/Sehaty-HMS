using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sehaty.Application.Dtos.IdentityDtos
{
    public class RefreshRequestDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
