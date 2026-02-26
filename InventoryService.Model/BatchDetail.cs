using System;
using System.Collections.Generic;

namespace InventoryService.Model;

public partial class BatchDetail
{
    public int BatchDetailId { get; set; }

    public int BatchId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }

    public DateOnly? ExpiredDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;
}
