using System;
using System.Collections.Generic;

namespace OrderService.Model;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }

    public decimal? DiscountPerItem { get; set; }

    public decimal Subtotal { get; set; }

    public int? Price { get; set; }

    public string? ProductName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
