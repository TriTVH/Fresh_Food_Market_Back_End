using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Service.DTO.ResponseObject
{
    public class AuthenticationToken
    {
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
        public AuthenticationToken(string token, DateTime expirationTime)
        {
            Token = token;
            ExpirationTime = expirationTime;
        }
    }
}
