using AuthService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Repository
{
    public interface IAccountRepository
    {
        public Task<Account?> IsValidUserCredential(string phone, string password);
    }
}
