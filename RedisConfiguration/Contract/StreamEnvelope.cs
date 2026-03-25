using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Contract
{
    public class StreamEnvelope
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString("N");
        public string MessageType { get; set; } = default!;
        public string Source { get; set; } = default!;
        public string SagaId { get; set; } = default!;
        public string CorrelationId { get; set; } = default!; // orderId
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string Payload { get; set; } = default!;
    
     }
}
