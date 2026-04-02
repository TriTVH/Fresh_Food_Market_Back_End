using OrderService.Service.DTO;
using OrderService.Service.DTO.External;
using OrderService.Service.DTO.External.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.HttpClients
{
    public class VoucherHttpClient : IVoucherHttpClient
    {
        private readonly HttpClient _httpClient;

        public VoucherHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<VoucherDetailResponse>> ApplyVoucherDetailAsync(CreateVoucherDetailRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/voucher-detail", request);
          

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<VoucherDetailResponse>>>();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(apiResponse.Message);
            }

            return apiResponse?.Data;
        }
        public async Task<bool> UnApplyVoucherDetailAsync(int orderId)
        {
            var response = await _httpClient.DeleteAsync($"/api/voucher-detail?orderId={orderId}");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(apiResponse?.Message ?? "Failed to unapply voucher detail");
            }

            return apiResponse?.Data ?? false;
        }
    }
}
