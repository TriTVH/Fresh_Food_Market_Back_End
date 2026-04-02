using OrderService.Model.Entities;

namespace OrderService.Repository
{
    public interface ITransactionRepo
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task UpdateAsync(Transaction transaction);

        Task DeleteAsync(Transaction transaction);
    }
}
