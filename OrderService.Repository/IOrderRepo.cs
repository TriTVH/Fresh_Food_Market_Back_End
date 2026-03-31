using OrderService.Model.Entities;

namespace OrderService.Repository
{
    public interface IOrderRepo
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task UpdateAsync(Order order);
    }
}
