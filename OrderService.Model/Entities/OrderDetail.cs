using System;
using System.Collections.Generic;

namespace OrderService.Model.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int Quantity { get; set; }
    public int MissingQuantity { get; set; }
    public int RefundQuantity { get; set; }
    public decimal RefundAmount { get; set; }

    public decimal? DiscountPerItem { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? Price { get; set; }

    public string? ProductName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Order Order { get; set; } = null!;
}
