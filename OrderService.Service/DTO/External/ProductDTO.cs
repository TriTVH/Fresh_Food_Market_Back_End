namespace OrderService.Service.DTO.External
{
    // Mirrors the response shape from ProductCatalogService ApiResponse<ProductDTO>
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal PriceSell { get; set; }
        public bool? IsAvailable { get; set; }
        public int? Quantity { get; set; }
    }
}
