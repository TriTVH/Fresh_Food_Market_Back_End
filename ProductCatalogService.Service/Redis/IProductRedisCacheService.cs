using ProductCatalogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.Redis
{
    public interface IProductRedisCacheService
    {
        Task<List<ProductDTO>> GetAllProductsFromRedisAsync();
        Task<ProductDTO?> GetProductByIdFromRedisAsync(int productId);
        Task SetAllProductsToRedisAsync(List<ProductDTO> products);
        Task SetProductToRedisAsync(ProductDTO product);
        Task RemoveProductFromRedisAsync(int productId);
        Task<bool> RedisKeyExistsAsync();
        Task ReloadAllProductsFromDbToRedisAsync();
    }
}
