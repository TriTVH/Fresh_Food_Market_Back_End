namespace VoucherService.Service.DTO.Response;

public class DiscountProgramResponse
{
    public int ProgramId { get; set; }
    public int? DiscountProduct { get; set; }
    public int DiscountFor { get; set; }
    public string TypeDiscount { get; set; } = string.Empty;
    public decimal? DiscountPercentage { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int? ValidTo { get; set; }
    public int? ValidFrom { get; set; }
    public int? MaxUsage { get; set; }
    public int? CurrentUsage { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}