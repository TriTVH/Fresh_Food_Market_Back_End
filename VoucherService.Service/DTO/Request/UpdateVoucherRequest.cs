using System.ComponentModel.DataAnnotations;

namespace VoucherService.Service.DTO.Request;

public class UpdateVoucherRequest
{
    [Required]
    public int VoucherId { get; set; }

    public int? AccountId { get; set; }

    [Required]
    [MaxLength(50)]
    public string VoucherCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? VoucherName { get; set; }

    public string? Description { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountMax { get; set; }

    public string? TypeDiscountTime { get; set; }
    public int? MaxUsage { get; set; }
    public int? CurrentUsage { get; set; }
    public int? ValidFrom { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}
