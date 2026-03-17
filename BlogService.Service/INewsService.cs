using BlogService.Service.DTO;
using BlogService.Service.DTO.Request;
using BlogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Service
{
    public interface INewsService
    {
        Task<ApiResponse<List<NewsDTO>>> GetNewsAsync();
        Task<ApiResponse<List<NewsDTO>>> GetPublishedNewsAsync();
        Task<ApiResponse<NewsDTO>> GetNewsByIdAsync(int newsId);
        Task<ApiResponse<NewsDTO>> CreateNewsAsync(NewsRequestDTO request);
        Task<ApiResponse<NewsDTO>> UpdateNewsAsync(int newsId, NewsRequestDTO request);
        Task<ApiResponse<bool>> DeleteNewsAsync(int newsId);
    }
}
