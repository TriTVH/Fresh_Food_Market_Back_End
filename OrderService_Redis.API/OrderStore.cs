using Microsoft.EntityFrameworkCore;
using OrderService_Redis.API.Entities;
using RedisConfiguration.DTOs;
using System.Collections.Concurrent;

namespace OrderService_Redis.API
{
    public class OrderStore
    {
            private readonly ConcurrentDictionary<string, OrderRecord> _orders = new();
            private readonly OrderSerRedisContext _context;

        public OrderStore()
        {
            _context = new();
        }

            public async Task<OrderRecord> Create(CreateOrderRequest request)
            {
                var orderId = $"ORD-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            var order = new OrderRecord
            {
                OrderId = orderId,
                OrderStatus = "PENDING",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
                Items = request.Items
            };
            var orderInDB = new Order
            {
                Id = orderId,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderItems = request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow,
                }).ToList()
            };

                _orders[order.OrderId] = order;

                 _context.Orders.Add(orderInDB);
                 await  _context.SaveChangesAsync();
                return order;
            }

        public async Task<OrderRecord?> Get(string orderId)
        {
            return await _context.Orders
                .Where(x => x.Id == orderId)
                .Select(x => new OrderRecord
                {
                    OrderId = x.Id,
                    OrderStatus = x.Status,
                    CreatedAtUtc = x.CreatedAt,
                    UpdatedAtUtc = x.UpdatedAt,
                    Items = x.OrderItems.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        CreatedAt = i.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderRecord>> GetAll()
        {
            return await _context.Orders
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new OrderRecord
                {
                    OrderId = x.Id,
                    OrderStatus = x.Status,
                    CreatedAtUtc = x.CreatedAt,
                    UpdatedAtUtc = x.UpdatedAt,
                    Items = x.OrderItems.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        CreatedAt = i.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }



        public bool CanMoveToPackaging(OrderRecord? order)
            {
                if (!string.Equals(order.OrderStatus, "PENDING", StringComparison.OrdinalIgnoreCase))
                    return false;

               return true;
        }

            public async Task MarkPackaging(string orderId)
            {
                if (_orders.TryGetValue(orderId, out var order))
                {
                    order.OrderStatus = "PACKAGING";
                    order.UpdatedAtUtc = DateTime.UtcNow;
                }
                var orderInDB = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId); 
                if (orderInDB != null)
            {
                orderInDB.Status = "PACKAGING";
            }
                 _context.Orders.Update(orderInDB);
                 await _context.SaveChangesAsync();
            }

            public async void MarkOutOfStock(string orderId)
            {
                if (_orders.TryGetValue(orderId, out var order))
                {
                    order.OrderStatus = "OUT_OF_STOCK";
                    order.UpdatedAtUtc = DateTime.UtcNow;
                }
            var orderInDB = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (orderInDB != null)
            {
                orderInDB.Status = "OUT_OF_STOCK";
            }
            _context.Orders.Update(orderInDB);
            await _context.SaveChangesAsync();
        }
    }
        

        public class OrderRecord
        {
            public string OrderId { get; set; } = default!;
    
            public string OrderStatus { get; set; } = default!;
            public List<OrderItemDTO> Items { get; set; } = [];
            public DateTime CreatedAtUtc { get; set; }
            public DateTime? UpdatedAtUtc { get; set; }
        }

    public sealed class CreateOrderRequest
    {
        public List<OrderItemDTO> Items { get; set; } = [];
    }

}
