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

                var productDTO = new ProductDTO()
                {
                    ProductId = created.ProductId,
                    ProductName = created.ProductName,
                    SubCategoryId = created.SubCategoryId,
                    SubCategoryName = created.SubCategory.SubCategoryName,
                    CategoryName = created.SubCategory.Category.CategoryName,
                    Description = created.Description,
                    PriceSell = created.PriceSell,
                    Quantity = created.Quantity,
                    Weight = created.Weight,
                    Unit = created.Unit,
                    IsOrganic = created.IsOrganic,
                    Certification = created.Certification,
                    IsAvailable = created.IsAvailable,
                    ImagesJson = created.ImagesJson,
                    RatingCount = created.RatingCount,
                    RatingAverage = created.RatingAverage,
                    SoldCount = created.SoldCount,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt
                };

                var productsFromRedis = await _productRedisCacheService.GetAllProductsFromRedisAsync();

                if (productsFromRedis == null || !productsFromRedis.Any())
                {
                    await _productRedisCacheService.ReloadAllProductsFromDbToRedisAsync();
                }
                else
                {
                    productsFromRedis.Add(productDTO);
                    await _productRedisCacheService.SetAllProductsToRedisAsync(productsFromRedis);
                }


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
                    SubCategoryId = p.SubCategoryId,
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
                    SubCategoryId = product.SubCategoryId,
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

        public Task<ApiResponse<ProductDTO>> UpdateProductAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<ProductDTO>> UpdateProductQtyAsync(UpdateProductQtyRequest request)
        {

            var product = await _productRepo.GetProductByIdAsync(request.ProductId);

            if (product == null)
            {
                return ApiResponse <ProductDTO>.Error(null, "Product not found or be deleted");
            }

            switch (request.Action)
            {
                case UpdateQtyAction.Add:
                    product.Quantity += request.ProductQty;
                    break;

                default:
                    return ApiResponse<ProductDTO>.Error(null, "Invalid Action");
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.UpdateAsync(product);

            var products = await _productRedisCacheService.GetAllProductsFromRedisAsync();

            if (products == null || !products.Any())
            {
                await _productRedisCacheService.ReloadAllProductsFromDbToRedisAsync();
            }
            else
            {
                var productInRedis = products.FirstOrDefault(p => p.ProductId == request.ProductId);
                productInRedis.Quantity = product.Quantity;
                await _productRedisCacheService.SetAllProductsToRedisAsync(products);
            }

            return ApiResponse<ProductDTO>.Ok(null, "Product quantity updated successfully");
        }
    }
}
