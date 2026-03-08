using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Contracts
{
    public record UpdateProductQtyCommand(Guid MessageId, int batchId, List<Item> items);
    public record Item(int productId, int quantity);

}
