using ProductCatalogService.Model;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;
using ProductCatalogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductDTO>>> GetAllProducts();
        Task<ApiResponse<ProductDTO>> CreateProduct(CreateProductModel request);
        Task<ApiResponse<ProductDTO>> GetProductByIdAsync(int productId);
        Task<ApiResponse<ProductDTO>> UpdateProductQtyAsync(UpdateProductQtyRequest request);
        Task<ApiResponse<ProductDTO>> UpdateProductAsync(UpdateProductModel request);
        Task<ApiResponse<List<ProductDTO>>> GetActiveProducts();
    }
}
