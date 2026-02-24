using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO.Response
{
    public class SupplierDTO
    {
        public int SupplierId { get; set; }

        public string Name { get; set; } = null!;

        public string Address { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public SupplierDTO(int supplierId, string name, string address, DateTime createdAt, DateTime updatedAt)
        {
            SupplierId = supplierId;
            Name = name;
            Address = address;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public SupplierDTO()
        {
        }
    }
}
