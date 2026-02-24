using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;
using ProductCatalogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service
{
    public interface ISupplierService
    {
        Task<ApiResponse<SupplierDTO>> CreateSupplierAsync(SupplierModel request);
    }
}
