using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts
{
    public record UpdateProductQtyReplied(Guid MessageId, int batchId, bool isSuccess, string message);
    
}
