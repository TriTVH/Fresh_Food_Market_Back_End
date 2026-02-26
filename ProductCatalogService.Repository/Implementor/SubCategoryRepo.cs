using Microsoft.EntityFrameworkCore;
using ProductCatalogService.Model;
using ProductCatalogService.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Repository.Implementor
{
    public class SubCategoryRepo : ISubCategoryRepo
    {
        ProductCatalogMgmtFfmContext _context;
        public SubCategoryRepo(ProductCatalogMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<List<SubCategory>> GetSubCategories()
        {
            return await _context.SubCategories
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<SubCategory> GetSubCategoryById(int subCategoryId)
        {
            return await _context.SubCategories
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.SubCategoryId == subCategoryId);
        }
    }
}
