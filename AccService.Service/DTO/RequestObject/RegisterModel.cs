using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.DTO.RequestObject
{
    public class RegisterModel
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public RegisterModel(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }
    }
}
