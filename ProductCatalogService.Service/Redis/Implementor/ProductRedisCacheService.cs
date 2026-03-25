using ProductCatalogService.Model.DBContext;
using ProductCatalogService.Repository;
using ProductCatalogService.Service.DTO.Response;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.Redis.Implementor
{
    public class ProductRedisCacheService : IProductRedisCacheService
    {
        private const string RedisKey = "products";

        private readonly IProductRepo _repo;
        private readonly IConnectionMultiplexer _redis;

        public ProductRedisCacheService(IProductRepo repo, IConnectionMultiplexer redis)
        {
            _repo = repo;
            _redis = redis;
        }

        public async Task<List<ProductDTO>> GetAllProductsFromRedisAsync()
        {
            var db = _redis.GetDatabase();

            var entries = await db.HashGetAllAsync(RedisKey);

            if (entries.Length == 0)
            {
                return new List<ProductDTO>();
            }

            return entries
                .Select(x => JsonSerializer.Deserialize<ProductDTO>(x.Value!))
                .Where(x => x != null)
                .Cast<ProductDTO>()
                .OrderBy(x => x.ProductId)
                .ToList();
        }

        public async Task<ProductDTO?> GetProductByIdFromRedisAsync(int productId)
        {
            var db = _redis.GetDatabase();

            var value = await db.HashGetAsync(RedisKey, productId.ToString());

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<ProductDTO>(value!);
        }

        public async Task SetAllProductsToRedisAsync(List<ProductDTO> products)
        {
            var db = _redis.GetDatabase();

            await db.KeyDeleteAsync(RedisKey);

            if (products == null || !products.Any())
            {
                return;
            }

            var entries = products
                .Select(x => new HashEntry(
                    x.ProductId.ToString(),
                    JsonSerializer.Serialize(x)))
                .ToArray();

            await db.HashSetAsync(RedisKey, entries);

            await db.KeyExpireAsync(RedisKey, TimeSpan.FromMinutes(15));
        }

        public async Task SetProductToRedisAsync(ProductDTO product)
        {
            var db = _redis.GetDatabase();

            await db.HashSetAsync(
                RedisKey,
                product.ProductId.ToString(),
                JsonSerializer.Serialize(product));

            await db.KeyExpireAsync(RedisKey, TimeSpan.FromMinutes(5));
        }

        public async Task RemoveProductFromRedisAsync(int productId)
        {
            var db = _redis.GetDatabase();

            await db.HashDeleteAsync(RedisKey, productId.ToString());
        }

        public async Task<bool> RedisKeyExistsAsync()
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(RedisKey);
        }

        public async Task ReloadAllProductsFromDbToRedisAsync()
        {
            var products = await _repo.GetAllProducts();

            var productDTOs = products.Select(p => new ProductDTO()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                SubCategoryId = p.SubCategoryId,
                SubCategoryName = p.SubCategory.SubCategoryName,
                CategoryName = p.SubCategory.Category.CategoryName,
                ManufacturingLocation = p.ManufacturingLocation,
                Description = p.Description,
                PriceSell = p.PriceSell,
                Quantity = p.Quantity,
                Weight = p.Weight,
                Unit = p.Unit,
                IsOrganic = p.IsOrganic,
                Certification = p.Certification,
                IsAvailable = p.IsAvailable,
                ImagesJson = p.ImagesJson,
                RatingCount = p.RatingCount,
                RatingAverage = p.RatingAverage,
                SoldCount = p.SoldCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            await SetAllProductsToRedisAsync(productDTOs);
        }
    }
}
