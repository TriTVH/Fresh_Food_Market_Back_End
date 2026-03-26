using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Contract
{
    public static class StreamConstants
    {
        public const string InventoryCommands = "inventory.commands";
        public const string VoucherCommands = "voucher.commands";
        public const string SagaReplies = "saga.replies";

        public const string InventoryGroup = "inventory-group";
        public const string VoucherGroup = "voucher-group";
        public const string OrderGroup = "order-group";
    }
}
