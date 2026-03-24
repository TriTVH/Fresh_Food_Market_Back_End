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

        Task<ApiResponse<ProductDTO>> UpdateProduct(UpdateProductModel request);
        Task<ApiResponse<bool>> DeleteProduct(int productId);
        // Hỗ trợ search giống UI FE
        Task<ApiResponse<List<ProductDTO>>> GetAllProducts(string? search = null);
    }
}
