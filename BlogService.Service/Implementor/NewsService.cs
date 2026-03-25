using BlogService.Repository;
using BlogService.Service.DTO;
using BlogService.Service.DTO.Request;
using BlogService.Service.DTO.Response;
using System.Net.Http;
using System.Net.Http.Json;
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
        HttpClient _httpClient;
        public NewsService(INewsRepository repo, HttpClient httpClient)
        {
            _repo = repo;
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<List<NewsDTO>>> GetNewsAsync()
        {
            var entities = await _repo.GetNewsAsync();
            var dtos = entities.Select(MapToDto).ToList();
            return ApiResponse<List<NewsDTO>>.Ok(dtos);
        }

        public async Task<ApiResponse<List<NewsDTO>>> GetPublishedNewsAsync()
        {
            var entities = await _repo.GetNewsAsync();
            var dtos = entities
                .Where(e => !string.IsNullOrEmpty(e.Status) && e.Status.Equals("Published", StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto)
                .ToList();
            return ApiResponse<List<NewsDTO>>.Ok(dtos);
        }

        public async Task<ApiResponse<NewsDTO>> GetNewsByIdAsync(int newsId)
        {
            var entity = await _repo.GetByIdAsync(newsId);
            if (entity == null)
            {
                return ApiResponse<NewsDTO>.Error(null!, "News not found", 404);
            }

            return ApiResponse<NewsDTO>.Ok(MapToDto(entity));
        }

        public async Task<ApiResponse<NewsDTO>> CreateNewsAsync(NewsRequestDTO request)
        {
            if (request == null)
            {
                return ApiResponse<NewsDTO>.Error(null!, "Request body is required", 400);
            }

            if (request.SubCategoryId.HasValue)
            {
                var exists = await SubCategoryExistsAsync(request.SubCategoryId.Value);
                if (!exists)
                {
                    return ApiResponse<NewsDTO>.Error(null!, "SubCategoryId is invalid", 400);
                }
            }

            var entity = new Model.News
            {
                SubCategoryId = request.SubCategoryId,
                Title = request.Title,
                Content = request.Content,
                Image = request.Image,
                Category = request.Category,
                Status = request.Status,
                PublishDate = request.PublishDate,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(entity);
            return ApiResponse<NewsDTO>.Ok(MapToDto(created), "Created", 201);
        }

        public async Task<ApiResponse<NewsDTO>> UpdateNewsAsync(int newsId, NewsRequestDTO request)
        {
            if (request == null)
            {
                return ApiResponse<NewsDTO>.Error(null!, "Request body is required", 400);
            }

            if (request.SubCategoryId.HasValue)
            {
                var exists = await SubCategoryExistsAsync(request.SubCategoryId.Value);
                if (!exists)
                {
                    return ApiResponse<NewsDTO>.Error(null!, "SubCategoryId is invalid", 400);
                }
            }

            var entity = await _repo.GetByIdAsync(newsId);
            if (entity == null)
            {
                return ApiResponse<NewsDTO>.Error(null!, "News not found", 404);
            }

            entity.Title = request.Title;
            entity.Content = request.Content;
            entity.Image = request.Image;
            entity.Category = request.Category;
            entity.Status = request.Status;
            entity.SubCategoryId = request.SubCategoryId;
            entity.PublishDate = request.PublishDate;
            entity.UpdatedDate = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(entity);
            return ApiResponse<NewsDTO>.Ok(MapToDto(updated), "Updated");
        }

        public async Task<ApiResponse<bool>> DeleteNewsAsync(int newsId)
        {
            var entity = await _repo.GetByIdAsync(newsId);
            if (entity == null)
            {
                return ApiResponse<bool>.Error(false, "News not found", 404);
            }

            await _repo.DeleteAsync(entity);
            return ApiResponse<bool>.Ok(true, "Deleted");
        }

        private static NewsDTO MapToDto(Model.News e)
        {
            return new NewsDTO
            {
                NewsId = e.NewsId,
                SubCategoryId = e.SubCategoryId,
                Title = e.Title,
                Content = e.Content,
                Image = e.Image,
                Category = e.Category,
                Status = e.Status,
                CreatedDate = e.CreatedDate,
                UpdatedDate = e.UpdatedDate,
                PublishDate = e.PublishDate,
            };
        }

        private async Task<bool> SubCategoryExistsAsync(int subCategoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/sub-category");
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var payload = await response.Content.ReadFromJsonAsync<ApiResponse<List<SubCategoryLookupDTO>>>();
                return payload?.Data?.Any(x => x.SubCategoryId == subCategoryId) == true;
            }
            catch
            {
                return false;
            }
        }

        private class SubCategoryLookupDTO
        {
            public int SubCategoryId { get; set; }
        }
    }
}
