using OrderService.Model.Entities;

namespace OrderService.Repository
{
    public interface IOrderRepo
    {
        Task<Order> CreateAsync(Order order);

        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<List<Order>> GetByUsernameAsync(string accUsername);
        Task<List<Order>> GetAllOrders();

    }
}
