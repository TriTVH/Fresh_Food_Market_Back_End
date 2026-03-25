using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.DTO.ResponseObject
{
    public class AuthenticationToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredAt { get; set; }
       
        public int Role { get; set; }

        public string Username { get; set; }
    }
}
