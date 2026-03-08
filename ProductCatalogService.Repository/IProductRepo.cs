using ProductCatalogService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Repository
{
    public interface IProductRepo
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<int> SaveChangeAsync(Product product);
        Task<Product> GetProductByIdAsync(int id);



    }
}
