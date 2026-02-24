using AuthService.Model;
using AuthService.Model.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Repository.Implementor
{
    public class AccountRepository : IAccountRepository
    {
        public AccountMgmtFfmContext _context;
        public AccountRepository(AccountMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<Account?> IsValidUserCredential(string phone, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Phone == phone && x.Password == password && x.IsActive == true);
        }
    }
}
