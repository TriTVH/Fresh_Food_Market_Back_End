using ProductCatalogService.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO.Request
{
    public class UpdateProductModel
    {

        [Required]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Sub Category Id is required")]
        public int SubCategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [MaxLength(200, ErrorMessage = "Product Name cannot exceed 200 characters")]
        public string ProductName { get; set; } = null!;

        [MaxLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Product quantity must be positive number")]
        public int ProductQty { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Manufacturing Location cannot exceed 255 characters")]
        public string? ManufacturingLocation { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "9999999999999.99",
    ErrorMessage = "Price must be greater than 0")]
        public decimal PriceSell { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "Weight must be greater than 0")]
        public decimal? Weight { get; set; }

        [Required]
        [RegularExpression("gram|kilogram|ton",
    ErrorMessage = "Invalid unit value")]

        public string? Unit { get; set; }

        public bool? IsOrganic { get; set; }

        [MaxLength(255, ErrorMessage = "Certification Image cannot exceed 255 characters")]
        public string? Certification { get; set; }

        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<ImageItem> ImagesJson { get; set; } = new List<ImageItem>();
    }
}
