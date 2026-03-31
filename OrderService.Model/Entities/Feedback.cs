using System;
using System.Collections.Generic;

namespace OrderService.Model.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int OrderDetailId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
