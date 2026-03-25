namespace OrderService.Service.DTO.Response
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<OrderDetailDTO> Items { get; set; } = new();
    }

    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public int? Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}
