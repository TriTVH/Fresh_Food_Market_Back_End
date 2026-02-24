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
    public class ProductRepo
    {
        ProductCatalogMgmtFfmContext _context;
        public ProductRepo(ProductCatalogMgmtFfmContext context)
        {
            _context = context;
        }
      
    }
}
