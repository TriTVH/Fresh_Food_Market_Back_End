using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoucherService.Model;

public partial class VoucherDetail
{
    public int VoucherDetailId { get; set; }

    public int OrderId { get; set; }

    public int VoucherId { get; set; }
    [NotMapped]
    public decimal DiscountAmount { get; set; }

    public DateTime? AppliedDate { get; set; }

    public virtual Voucher Voucher { get; set; } = null!;
}
