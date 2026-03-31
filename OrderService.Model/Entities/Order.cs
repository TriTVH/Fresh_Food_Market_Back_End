using System;
using System.Collections.Generic;

namespace OrderService.Model.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public string AccountUsername { get; set; } = null!;

    public int? SubscriptionId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public string? ShippingName { get; set; }

    public string? ShippingPhone { get; set; }

    public string? ShippingAddress { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? ConfirmedDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public DateTime? CancelledDate { get; set; }

    public string? CancelReason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual SubscriptionTicket? Subscription { get; set; }
}
