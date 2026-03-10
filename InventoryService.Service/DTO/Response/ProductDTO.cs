using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryService.Service.DTO.Response
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string SubCategoryName { get; set; }

        public string CategoryName { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public string? ManufacturingLocation { get; set; }

        public decimal PriceSell { get; set; }

        public int? Quantity { get; set; }

        public decimal? Weight { get; set; }

        public string? Unit { get; set; }

        public bool? IsOrganic { get; set; }

        public string? Certification { get; set; }

        public bool? IsAvailable { get; set; }

        public int? SoldCount { get; set; }

        public decimal? RatingAverage { get; set; }

        public int? RatingCount { get; set; }
        public List<ImageItem> ImagesJson { get; set; } = new List<ImageItem>();

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }

    public class ImageItem
    {
        [Required]
        [JsonPropertyName("url")]
        public string Url { get; set; } = "";

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }
    }

}
