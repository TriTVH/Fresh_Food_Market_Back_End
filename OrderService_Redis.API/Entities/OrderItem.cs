using System;
using System.Collections.Generic;

namespace OrderService_Redis.API.Entities;

public partial class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string OrderId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
