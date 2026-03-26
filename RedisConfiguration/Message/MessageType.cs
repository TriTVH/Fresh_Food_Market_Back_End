using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Message
{
    public class MessageType
    {
        public const string ReserveInventory = "ReserveInventory";
        public const string InventoryReserved = "InventoryReserved";
        public const string InventoryReserveFailed = "InventoryReserveFailed";
    }
}
