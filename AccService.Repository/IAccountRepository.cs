using AccService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Repository
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAccounts();
        Task<Account?> CreateAccountAsync(Account account);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<Account?> IsValidUserCredential(string phone, string password);

        Task<bool> CheckPhoneExistsAsync(string phone);
        Task<bool> CheckUsernameExistsAsync(string username);
    }
}
