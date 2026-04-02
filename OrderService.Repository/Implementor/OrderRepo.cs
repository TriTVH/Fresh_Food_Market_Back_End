using Microsoft.EntityFrameworkCore;
using OrderService.Model.DBContext;
using OrderService.Model.Entities;

namespace OrderService.Repository.Implementor
{
    public class OrderRepo : IOrderRepo
    {
        private readonly OrderMgmtFfmContext _context;

        public OrderRepo(OrderMgmtFfmContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }


        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public Task<List<Order>> GetByUsernameAsync(string accUsername)
        {
            return _context.Orders
                .Where(o => o.AccountUsername == accUsername)
                .Include(o => o.OrderDetails)
                .Include(o => o.Transactions)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Transactions)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order?> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public Task<List<Order>> GetAllOrders()
        {
            return _context.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Transactions)
            .ToListAsync();
        }
    }
}
