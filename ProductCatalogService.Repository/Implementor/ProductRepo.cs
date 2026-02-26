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
            await _context.SaveChangesAsync();   
            return entry.Entity;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.SubCategory)
                .ThenInclude(sc => sc.Category)
                .ToListAsync();
        }
    }
}
