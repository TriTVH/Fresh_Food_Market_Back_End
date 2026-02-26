using ProductCatalogService.Repository;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.Implementor
{
    public class SubCategoryService : ISubCategoryService
    {
        ISubCategoryRepo _subCategoryRepo;
        public SubCategoryService(ISubCategoryRepo subCategoryRepo)
        {
            _subCategoryRepo = subCategoryRepo;
        }

        public async Task<ApiResponse<List<SubCategoryDTO>>> GetSubCategories()
        {
            try
            {
                var subCategories = await _subCategoryRepo.GetSubCategories();
                var subCategoryDTOs = subCategories.Select(sc => new SubCategoryDTO
                {
                    SubCategoryId = sc.SubCategoryId,
                    CategoryName = sc.Category.CategoryName,
                    SubCategoryName = sc.SubCategoryName
                }).ToList();
            return ApiResponse<List<SubCategoryDTO>>.Ok(subCategoryDTOs);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SubCategoryDTO>>.Error(null, ex.Message, 500);
            }
        }
    }
}
