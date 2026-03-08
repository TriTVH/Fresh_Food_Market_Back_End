using BlogService.Repository;
using BlogService.Service.DTO;
using BlogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Service.Implementor
{
    public class NewsService : INewsService
    {
        INewsRepository _repo;
        public NewsService(INewsRepository repo)
        {
            _repo = repo;
        }
        public async Task<ApiResponse<List<NewsDTO>>> GetNewsAsync()
        {
            var entities = await _repo.GetNewsAsync();
            var dtos = entities.Select(e => new NewsDTO
            {
                NewsId = e.NewsId,
                Title = e.Title,
                Content = e.Content,
                Image = e.Image,
                Category = e.Category,
                Status = e.Status,
                CreatedDate = e.CreatedDate,
                UpdatedDate = e.UpdatedDate,
                PublishDate = e.PublishDate,
                StaffId = e.StaffId,
            }).ToList();
            return ApiResponse<List<NewsDTO>>.Ok(dtos);
        }
    }
}
