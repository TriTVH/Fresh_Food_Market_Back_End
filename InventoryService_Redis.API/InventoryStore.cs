using RedisConfiguration.DTOs;
using System.Collections.Concurrent;

namespace InventoryService_Redis.API
{
    public class InventoryStore
    {
        private readonly ConcurrentDictionary<int, InventoryItem> _stocks = new();
        private readonly ConcurrentDictionary<string, ReservationRecord> _reservations = new();
        private readonly ConcurrentDictionary<string, byte> _processedMessages = new();

        public InventoryStore()
        {
            _stocks[1] = new InventoryItem { ProductId = 1, AvailableQty = 10, ReservedQty = 0 };
            _stocks[2] = new InventoryItem { ProductId = 2, AvailableQty = 5, ReservedQty = 0 };
            _stocks[3] = new InventoryItem { ProductId = 3, AvailableQty = 20, ReservedQty = 0 };
        }

        public IEnumerable<InventoryItem> GetStocks()
            => _stocks.Values.OrderBy(x => x.ProductId);

        public IEnumerable<ReservationRecord> GetReservations()
            => _reservations.Values.OrderByDescending(x => x.CreatedAtUtc);

        public bool TryMarkProcessed(string messageId)
            => _processedMessages.TryAdd(messageId, 1);

        public bool Reserve(string orderId, List<OrderItemDTO> items, out string reservationId, out string? reason)
        {
            lock (this)
            {
                if (_reservations.TryGetValue(orderId, out var existing))
                {
                    reservationId = existing.ReservationId;
                    reason = existing.Status == "RESERVED" ? null : "ALREADY_RELEASED";
                    return existing.Status == "RESERVED";
                }

                foreach (var item in items)
                {
                    if (!_stocks.TryGetValue(item.ProductId, out var stock))
                    {
                        reservationId = string.Empty;
                        reason = $"PRODUCT_{item.ProductId}_NOT_FOUND";
                        return false;
                    }

                    if (stock.AvailableQty < item.Quantity)
                    {
                        reservationId = string.Empty;
                        reason = "INSUFFICIENT_STOCK";
                        return false;
                    }
                }

                foreach (var item in items)
                {
                    var stock = _stocks[item.ProductId];
                    stock.AvailableQty -= item.Quantity;
                    stock.ReservedQty += item.Quantity;
                }

                reservationId = $"RSV-{Guid.NewGuid():N}";
                _reservations[orderId] = new ReservationRecord
                {
                    OrderId = orderId,
                    ReservationId = reservationId,
                    Items = items,
                    Status = "RESERVED",
                    CreatedAtUtc = DateTime.UtcNow
                };

                reason = null;
                return true;
            }
        }
    }

    public sealed class InventoryItem
    {
        public int ProductId { get; set; }
        public int AvailableQty { get; set; }
        public int ReservedQty { get; set; }
    }

    public sealed class ReservationRecord
    {
        public string OrderId { get; set; } = default!;
        public string ReservationId { get; set; } = default!;
        public string Status { get; set; } = default!;
        public List<OrderItemDTO> Items { get; set; } = [];
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
