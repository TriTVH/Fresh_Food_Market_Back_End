using System;

namespace OrderService.Model.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? Direction { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
