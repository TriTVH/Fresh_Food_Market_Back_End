using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMqConstants
    {
        public const string BatchExchange = "batch.exchange";

        // queues
        public const string ProductCommandQueue = "product.command.q";
        public const string BatchSagaReplyQueue = "batch.saga.reply.q";

        // routing keys
        public const string RK_ProductUpdateQtyCommand = "product.update-qty.command";
        public const string RK_ProductUpdateQtyReply = "product.update-qty.reply";
    }
}
