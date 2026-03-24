using System;
using System.Collections.Generic;

namespace VoucherService.Model;

public partial class VoucherDetail
{
    public int VoucherDetailId { get; set; }

    public int OrderId { get; set; }

    public int VoucherId { get; set; }

    public DateTime? AppliedDate { get; set; }

    public virtual Voucher Voucher { get; set; } = null!;
}
