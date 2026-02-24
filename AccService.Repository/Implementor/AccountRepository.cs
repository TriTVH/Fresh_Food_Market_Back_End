using AccService.Model;
using AccService.Model.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccService.Repository.Implementor
{
    public class AccountRepository : IAccountRepository
    {
        public AccountMgmtFfmContext _context;
        public AccountRepository(AccountMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<List<Account>> GetAccounts()
        {
            return await _context.Accounts
                .OrderByDescending(x => x.UpdatedDate)
                .Include(x => x.Role).ToListAsync();
        }
        public async Task<Account?> CreateAccountAsync(Account account)
        {
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.Phone == account.Phone);
            if (existingAccount != null)
            {
                return null; // Account with the same phone number already exists
            }
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Accounts.AnyAsync(x => x.Username == username);
        }
        public async Task<Account?> IsValidUserCredential(string phone, string password)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.Phone == phone && x.Password == password && x.IsActive == true);
        }
    }
}
