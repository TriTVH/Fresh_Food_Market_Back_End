using System;
using System.Collections.Generic;

namespace InventoryService.Model;

public partial class Batch
{
    public int BatchId { get; set; }

    public int SupplierId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string BatchCode { get; set; } = null!;

    public int? TotalItems { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public string? ImageConfirmReceived { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<BatchDetail> BatchDetails { get; set; } = new List<BatchDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
