using OrderService.Service.DTO.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.DTO.Request
{
    public class UpdateOrderModel
    {
        public int OrderId { get; set; }
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public OrderAction Action { get; set; }
        public List<UpdateOrderDetailModel> OrderDetails { get; set; } = new();
    }
    public class UpdateOrderDetailModel
    {

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int RefundAmount { get; set; }

    }
}
