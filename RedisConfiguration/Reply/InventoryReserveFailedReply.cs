using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Reply
{
    public class InventoryReserveFailedReply
    {
        public string SagaId { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public string Reason { get; set; } = default!;
    }
}
