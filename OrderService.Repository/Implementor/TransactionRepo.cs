using Microsoft.EntityFrameworkCore;
using OrderService.Model.DBContext;
using OrderService.Model.Entities;

namespace OrderService.Repository.Implementor
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly OrderMgmtFfmContext _context;

        public TransactionRepo(OrderMgmtFfmContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Order)
                .ToListAsync();
        }
    }
}
