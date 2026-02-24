using System;
using System.Collections.Generic;

namespace ProductCatalogService.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public int SubCategoryId { get; set; }

    public int? SupplierId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ManufacturingLocation { get; set; }

    public decimal PriceSell { get; set; }

    public int? Quantity { get; set; }

    public decimal? Weight { get; set; }

    public string? Unit { get; set; }

    public bool? IsOrganic { get; set; }

    public string? Certification { get; set; }

    public string? Images { get; set; }

    public bool? IsAvailable { get; set; }

    public int? SoldCount { get; set; }

    public decimal? RatingAverage { get; set; }

    public int? RatingCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SubCategory SubCategory { get; set; } = null!;

    public virtual Supplier? Supplier { get; set; }
}
