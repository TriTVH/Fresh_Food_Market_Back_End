namespace VoucherService.Service.DTO.Response;

public class VoucherDetailResponse
{
    public int VoucherDetailId { get; set; }
    public int OrderId { get; set; }
    public decimal DiscountAmount { get; set; }
    public int VoucherId { get; set; }
    public string VoucherCode { get; set; } = string.Empty;
    public DateTime? AppliedDate { get; set; }
}