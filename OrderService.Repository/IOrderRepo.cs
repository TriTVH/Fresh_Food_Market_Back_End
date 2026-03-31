using OrderService.Model;
using OrderService.Model.Entities;

namespace OrderService.Repository
{
    public interface IOrderRepo
    {
        //Task<List<Order>> GetAllAsync();
        Task<Order> CreateAsync(Order order);
        //Task<Order?> GetByIdAsync(int orderId);
        //Task UpdateAsync(Order order);
    }
}