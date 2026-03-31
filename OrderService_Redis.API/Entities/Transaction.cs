namespace OrderService_Redis.API.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string OrderId { get; set; } = null!;
    public string? Type { get; set; }
    public string? Direction { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Order? Order { get; set; }
}
