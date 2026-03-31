using System;
using System.Collections.Generic;

namespace OrderService.Model.Entities;

public partial class SubscriptionTicket
{
    public int SubscriptionId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Frequency { get; set; }

    public string? Status { get; set; }

    public int? TotalOrders { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
