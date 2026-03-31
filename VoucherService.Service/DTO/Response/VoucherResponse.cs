namespace VoucherService.Service.DTO.Response;

public class VoucherResponse
{
    public int VoucherId { get; set; }
    public int? AccountId { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
    public string? VoucherName { get; set; }
    public string? Description { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? TypeDiscountTime { get; set; }
    public int? MaxUsage { get; set; }
    public int? CurrentUsage { get; set; }
    public int? ValidFrom { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? DiscountFor { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
