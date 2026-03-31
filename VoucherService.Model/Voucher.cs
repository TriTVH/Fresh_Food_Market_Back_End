using System;
using System.Collections.Generic;

namespace VoucherService.Model;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string VoucherCode { get; set; } = null!;

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

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
}
