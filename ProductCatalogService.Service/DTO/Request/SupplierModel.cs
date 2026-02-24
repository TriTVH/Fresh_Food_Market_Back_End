using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO.Request
{
    public class SupplierModel
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public SupplierModel(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
