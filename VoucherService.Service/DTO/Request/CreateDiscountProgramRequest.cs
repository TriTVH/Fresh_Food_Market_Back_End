using System.ComponentModel.DataAnnotations;

namespace VoucherService.Service.DTO.Request;

public class CreateDiscountProgramRequest
{
    public int? DiscountProduct { get; set; }

    [Required]
    public int DiscountFor { get; set; }

    [Required]
    [MaxLength(50)]
    public string TypeDiscount { get; set; } = string.Empty;

    public decimal? DiscountPercentage { get; set; }

    [Required]
    public DateOnly FromDate { get; set; }

    [Required]
    public DateOnly ToDate { get; set; }

    public int? ValidTo { get; set; }
    public int? ValidFrom { get; set; }
    public int? MaxUsage { get; set; }
    public string? Status { get; set; }
}