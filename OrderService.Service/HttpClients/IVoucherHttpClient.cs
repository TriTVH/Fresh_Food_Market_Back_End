using OrderService.Service.DTO.External;
using OrderService.Service.DTO.External.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.HttpClients
{
    public interface IVoucherHttpClient
    {
        Task<List<VoucherDetailResponse>> ApplyVoucherDetailAsync(CreateVoucherDetailRequest request);
    }
}
