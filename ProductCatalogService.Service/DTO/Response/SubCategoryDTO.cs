using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO.Response
{
    public class SubCategoryDTO
    {
        public int SubCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string SubCategoryName { get; set; } = null!;
    }
}
