using ProductCatalogService.Model;
using ProductCatalogService.Repository;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;
using ProductCatalogService.Service.DTO.Response;
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
        public ProductService(IProductRepo productRepo, ISubCategoryRepo subCategoryRepo)
        {
            _productRepo = productRepo;
            _subCategoryRepo = subCategoryRepo;
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
                return ApiResponse<List<ProductDTO>>.Ok(productDTOs);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ProductDTO>>.Error(null, ex.Message, 500);
            }
        }
    }
}
