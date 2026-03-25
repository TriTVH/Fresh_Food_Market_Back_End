using RedisConfiguration.DTOs;
using System.Collections.Concurrent;

namespace OrderService_Redis.API
{
    public class OrderStore
    {
            private readonly ConcurrentDictionary<string, OrderRecord> _orders = new();

            public OrderRecord Create(CreateOrderRequest request)
            {
                var orderId = $"ORD-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

                var order = new OrderRecord
                {
                    OrderId = orderId,
                    UserId = request.UserId,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = request.PaymentStatus,
                    OrderStatus = "PENDING",
                    Items = request.Items,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _orders[order.OrderId] = order;
                return order;
            }

            public OrderRecord? Get(string orderId)
                => _orders.TryGetValue(orderId, out var order) ? order : null;

            public IEnumerable<OrderRecord> GetAll()
                => _orders.Values.OrderByDescending(x => x.CreatedAtUtc);

            public bool CanMoveToPackaging(OrderRecord order)
            {
                if (!string.Equals(order.OrderStatus, "PENDING", StringComparison.OrdinalIgnoreCase))
                    return false;

               return true;
        }

            public void MarkPackaging(string orderId)
            {
                if (_orders.TryGetValue(orderId, out var order))
                {
                    order.OrderStatus = "PACKAGING";
                    order.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            public void MarkOutOfStock(string orderId)
            {
                if (_orders.TryGetValue(orderId, out var order))
                {
                    order.OrderStatus = "OUT_OF_STOCK";
                    order.UpdatedAtUtc = DateTime.UtcNow;
                }
            }
        }

        public class OrderRecord
        {
            public string OrderId { get; set; } = default!;
            public string UserId { get; set; } = default!;
            public string PaymentMethod { get; set; } = default!;
            public string PaymentStatus { get; set; } = default!;
            public string OrderStatus { get; set; } = default!;
            public List<OrderItemDTO> Items { get; set; } = [];
            public DateTime CreatedAtUtc { get; set; }
            public DateTime? UpdatedAtUtc { get; set; }
        }

    public sealed class CreateOrderRequest
    {
        public string UserId { get; set; } = default!;
        public string PaymentMethod { get; set; } 
        public string PaymentStatus { get; set; }
        public List<OrderItemDTO> Items { get; set; } = [];
    }

}
