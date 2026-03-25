using System;
using System.Collections.Generic;

namespace OrderService.Model;

public partial class Order
{
    public int OrderId { get; set; }

    public int AccountId { get; set; }

    public int? SubscriptionId { get; set; }

    public int? VoucherId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public string? ShippingName { get; set; }

    public string? ShippingPhone { get; set; }

    public string? ShippingAddress { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

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
}
