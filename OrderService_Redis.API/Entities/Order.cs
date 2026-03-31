using System;
using System.Collections.Generic;

namespace OrderService_Redis.API.Entities;

public partial class Order
{
    public string Id { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
