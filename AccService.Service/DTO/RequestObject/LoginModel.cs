using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Service.DTO.RequestObject
{
    public class LoginModel
    {
        [Phone]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public LoginModel(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }
    }
}
