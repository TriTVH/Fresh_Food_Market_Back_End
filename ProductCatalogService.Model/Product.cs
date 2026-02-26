using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductCatalogService.Model;

public partial class Product
{
    public int ProductId { get; set; }

    public int SubCategoryId { get; set; }

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

    [NotMapped]
    public List<ImageItem> ImagesJson
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Images))
                return new List<ImageItem>();

            return JsonSerializer.Deserialize<List<ImageItem>>(
                Images,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<ImageItem>();
        }
        set
        {
            Images = value == null || value.Count == 0
                ? null
                : JsonSerializer.Serialize(value);
        }
    }
    public bool? IsAvailable { get; set; }

    public int? SoldCount { get; set; }

    public decimal? RatingAverage { get; set; }

    public int? RatingCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SubCategory SubCategory { get; set; } = null!;
}

public class ImageItem
{
    [Required]
    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}
