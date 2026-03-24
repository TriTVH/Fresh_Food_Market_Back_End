using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProductCatalogService.Model;
using ProductCatalogService.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProductCatalogService.Repository.Implementor
{
    public class ProductRepo : IProductRepo
    {
        ProductCatalogMgmtFfmContext _context;
     
        public ProductRepo(ProductCatalogMgmtFfmContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var entry = await _context.Products.AddAsync(product);
            
            return entry.Entity;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.SubCategory)
                .ThenInclude(sc => sc.Category)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.SubCategory)
                .ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public Task<int> SaveChangeAsync(Product product)
        {
            return _context.SaveChangesAsync();
        }

   
        public async Task<Product> UpdateAsync(Product product)
        {
            var entry = _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            product.IsAvailable = false;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
