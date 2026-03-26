using RedisConfiguration.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.Command
{
    public class ReserveInventoryCommand
    {
        public string SagaId { get; set; } = default!;
        public string OrderId { get; set; } = default!;
        public List<OrderItemDTO> Items { get; set; } = [];
    }
}
