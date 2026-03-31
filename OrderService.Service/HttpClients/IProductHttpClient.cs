using OrderService.Service.DTO.External;

namespace OrderService.Service.HttpClients
{
    public interface IProductHttpClient
    {
        Task<ProductDTO?> GetProductByIdAsync(int productId);
    }
}
