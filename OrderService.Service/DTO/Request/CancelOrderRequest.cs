using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.DTO.Request
{
    public class CancelOrderRequest
    {
        public int OrderId { get; set; }
        public string CancelReason { get; set; }
    }
}
