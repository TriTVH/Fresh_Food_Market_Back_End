using BlogService.Service.DTO;
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
    }
}
