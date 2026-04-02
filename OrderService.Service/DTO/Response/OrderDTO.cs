namespace OrderService.Service.DTO.Response
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string? Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }

        public string? ShippingPhone { get; set; }
        public string? ShippingName { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }


        public DateTime CreatedDate { get; set; }
        public string? TransactionStatus { get; set; }
        public List<OrderDetailDTO> Items { get; set; } = new();
    }

    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public decimal DiscountPerItem { get; set; }

        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}
