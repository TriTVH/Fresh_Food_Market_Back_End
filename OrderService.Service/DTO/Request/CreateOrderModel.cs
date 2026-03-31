using System.ComponentModel.DataAnnotations;

namespace OrderService.Service.DTO.Request
{
    public class CreateOrderModel
    {
        public List<int> VoucherIds { get; set; } = new();
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal? ShippingFee { get; set; }
        public string? PaymentMethod { get; set; }
        public List<CreateOrderDetailModel> Items { get; set; } = new();
    }

    public class CreateOrderDetailModel
    {

        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }

    }
}
