using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Reply
{
    public class InventoryReservedReply
    {
        public string SagaId { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public string ReservationId { get; set; } = default!;
    }
}
