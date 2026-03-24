using System.ComponentModel.DataAnnotations;

namespace ProductCatalogService.Service.DTO.Request
{
    public class UpdateProductModel : CreateProductModel
    {
        [Required(ErrorMessage = "Product Id is required for updating")]
        public int ProductId { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
