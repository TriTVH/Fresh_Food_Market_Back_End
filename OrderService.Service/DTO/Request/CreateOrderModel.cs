namespace OrderService.Service.DTO.Request
{
    public class CreateOrderModel
    {
        public int AccountId { get; set; }
        public int? SubscriptionId { get; set; }
        public int? VoucherId { get; set; }
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal? ShippingFee { get; set; }
        public List<CreateOrderDetailModel> Items { get; set; } = new();
    }

    public class CreateOrderDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
