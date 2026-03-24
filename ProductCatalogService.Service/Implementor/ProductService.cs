using Microsoft.EntityFrameworkCore;
using ProductCatalogService.Model;
using ProductCatalogService.Model.DBContext;
using ProductCatalogService.Repository;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;
using ProductCatalogService.Service.DTO.Response;
using ProductCatalogService.Service.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.Implementor
{
    public class ProductService : IProductService
    {
        IProductRepo _productRepo;
        ISubCategoryRepo _subCategoryRepo;
        IProductRedisCacheService _productRedisCacheService;

        public ProductService(IProductRedisCacheService productRedisCacheService, IProductRepo productRepo, ISubCategoryRepo subCategoryRepo)
        {
            _productRepo = productRepo;
            _subCategoryRepo = subCategoryRepo;
            _productRedisCacheService = productRedisCacheService;
        }

        public async Task<ApiResponse<ProductDTO>> CreateProduct(CreateProductModel request)
        {
            try
            {
                var subCategory = await _subCategoryRepo.GetSubCategoryById(request.SubCategoryId);
                if (subCategory == null)
                {
                    return ApiResponse<ProductDTO>.Error(null, "Sub Category not found", 404);
                }
                if (request.ImagesJson.Count == 1)
                {
                    var image = request.ImagesJson[0];
                    image.Primary = true;
                }
                else
                {
                    var primaryImages = request.ImagesJson.Where(x => x.Primary).ToList();

                    if (primaryImages.Count == 0)
                    {
                        // Không có ảnh nào primary → auto set ảnh đầu tiên
                        request.ImagesJson[0].Primary = true;
                    }
                    else if (primaryImages.Count > 1)
                    {
                       return ApiResponse<ProductDTO>.Error(null, "Only one image can be set as primary", 400);
                    }
                }

                var product = new Product
                {
                    SubCategoryId = request.SubCategoryId,
                    ProductName = request.ProductName,
                    Description = request.Description,
                    ManufacturingLocation = request.ManufacturingLocation,
                    PriceSell = request.PriceSell,
                    Quantity = 0,
                    Weight = request.Weight,
                    Unit = request.Unit,
                    IsOrganic = request.IsOrganic,
                    Certification = request.Certification,
                    IsAvailable = true,
                    ImagesJson = request.ImagesJson,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var created = await _productRepo.CreateAsync(product);
                return ApiResponse<ProductDTO>.Ok(null, "Product created successfully", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.Error(null, ex.Message, 500);
            }
        }
        public async Task<ApiResponse<List<ProductDTO>>> GetAllProducts()
        {
            try
            {
                var productsFromRedis = await _productRedisCacheService.GetAllProductsFromRedisAsync();

                if (productsFromRedis.Any())
                {
                    return ApiResponse<List<ProductDTO>>.Ok(productsFromRedis);
                }

                var products = await _productRepo.GetAllProducts();
                var productDTOs = products.Select(p => new ProductDTO()
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    SubCategoryName = p.SubCategory.SubCategoryName,
                    CategoryName = p.SubCategory.Category.CategoryName,
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
                await _productRedisCacheService.SetAllProductsToRedisAsync(productDTOs);
                return ApiResponse<List<ProductDTO>>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<ProductDTO>> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _productRepo.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return ApiResponse<ProductDTO>.Error(null, "Product not found", 404);
                }
                var productDTO = new ProductDTO()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    SubCategoryName = product.SubCategory.SubCategoryName,
                    CategoryName = product.SubCategory.Category.CategoryName,
                    Description = product.Description,
                    PriceSell = product.PriceSell,
                    Quantity = product.Quantity,
                    Weight = product.Weight,
                    Unit = product.Unit,
                    IsOrganic = product.IsOrganic,
                    Certification = product.Certification,
                    IsAvailable = product.IsAvailable,
                    ImagesJson = product.ImagesJson,
                    RatingCount = product.RatingCount,
                    RatingAverage = product.RatingAverage,
                    SoldCount = product.SoldCount,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };
                return ApiResponse<ProductDTO>.Ok(productDTO);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductDTO>.Error(null, ex.Message, 500);
            }
        }
        public async Task<ApiResponse<ProductDTO>> UpdateProduct(UpdateProductModel request)
        {
            var existingProduct = await _productRepo.GetProductByIdAsync(request.ProductId);
            if (existingProduct == null)
            {
                return ApiResponse<ProductDTO>.Error(null, "Không tìm thấy sản phẩm để cập nhật", 404);
            }

            existingProduct.SubCategoryId = request.SubCategoryId;
            existingProduct.ProductName = request.ProductName;
            existingProduct.Description = request.Description;
            existingProduct.ManufacturingLocation = request.ManufacturingLocation;
            existingProduct.PriceSell = request.PriceSell;
            existingProduct.Weight = request.Weight;
            existingProduct.Unit = request.Unit;
            existingProduct.IsOrganic = request.IsOrganic;
            existingProduct.Certification = request.Certification;
            existingProduct.ImagesJson = request.ImagesJson;
            existingProduct.IsAvailable = request.IsAvailable ?? existingProduct.IsAvailable;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(existingProduct);

            var productDto = MapToDTO(existingProduct);

            await _productRedisCacheService.SetProductToRedisAsync(productDto);

            return ApiResponse<ProductDTO>.Ok(productDto, "Cập nhật thành công", 200);
        }

        public async Task<ApiResponse<bool>> DeleteProduct(int productId)
        {
            var isDeleted = await _productRepo.DeleteAsync(productId);
            if (!isDeleted)
            {
                return ApiResponse<bool>.Error(false, "Sản phẩm không tồn tại hoặc đã bị xóa", 404);
            }

            await _productRedisCacheService.RemoveProductFromRedisAsync(productId);

            return ApiResponse<bool>.Ok(true, "Xóa sản phẩm thành công", 200);
        }
        private ProductDTO MapToDTO(Product p)
        {
            return new ProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                SubCategoryName = p.SubCategory?.SubCategoryName,
                CategoryName = p.SubCategory?.Category?.CategoryName,
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
            };
        }
        public async Task<ApiResponse<List<ProductDTO>>> GetAllProducts(string? search = null)
        {
            var cachedProducts = await _productRedisCacheService.GetAllProductsFromRedisAsync();

            if (cachedProducts == null || !cachedProducts.Any())
            {
                await _productRedisCacheService.ReloadAllProductsFromDbToRedisAsync();
                cachedProducts = await _productRedisCacheService.GetAllProductsFromRedisAsync();
            }

            var result = cachedProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                result = result.Where(p =>
                    (p.ProductName != null && p.ProductName.ToLower().Contains(keyword)) ||
                    (p.ProductId.ToString().Contains(keyword)) 
                );
            }

            return ApiResponse<List<ProductDTO>>.Ok(result.ToList(), "Lấy dữ liệu thành công", 200);
        }
    }
}
