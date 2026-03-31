using OrderService_Redis.API.Entities;
using RedisConfiguration.DTOs;
using System.Collections.Concurrent;

namespace OrderService_Redis.API
{
    public class SagaStore
    {
        private readonly ConcurrentDictionary<string, SagaRecord> _sagas = new();
        private readonly ConcurrentDictionary<string, byte> _processedMessages = new();

        public SagaRecord CreateFromOrder(OrderRecord order)
        {
            var saga = new SagaRecord
            {
                SagaId = $"SAGA-{Guid.NewGuid():N}",
                OrderId = order.OrderId,
                SagaStatus = "WAITING_INVENTORY",
                CurrentStep = "RESERVE_INVENTORY",
                CreatedAtUtc = DateTime.UtcNow
            };
            order.OrderStatus = "RESERVING_INVENTORY";
            _sagas[saga.SagaId] = saga;
            return saga;
        }

        public IEnumerable<SagaRecord> GetAll()
            => _sagas.Values.OrderByDescending(x => x.CreatedAtUtc);

        public SagaRecord? GetBySagaId(string sagaId)
            => _sagas.TryGetValue(sagaId, out var saga) ? saga : null;

        public SagaRecord? GetByOrderId(string orderId)
            => _sagas.Values.FirstOrDefault(x => x.OrderId == orderId);

        public bool TryMarkProcessed(string messageId)
            => _processedMessages.TryAdd(messageId, 1);

        public void MarkInventoryReserved(string sagaId, string reservationId)
        {
            if (_sagas.TryGetValue(sagaId, out var saga))
            {
                saga.ReservationId = reservationId;
                saga.SagaStatus = "COMPLETED";
                saga.CurrentStep = "DONE";
                saga.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        public void MarkInventoryFailed(string sagaId, string reason)
        {
            if (_sagas.TryGetValue(sagaId, out var saga))
            {
                saga.FailureReason = reason;
                saga.SagaStatus = "FAILED";
                saga.CurrentStep = "FAILED";
                saga.UpdatedAtUtc = DateTime.UtcNow;
            }
        }
    }

    public sealed class SagaRecord
    {
        public string SagaId { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public string SagaStatus { get; set; } = default!;
        public string CurrentStep { get; set; } = default!;
        public string? ReservationId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
