using BlogService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Repository
{
    public interface INewsRepository
    {
        Task<List<News>> GetNewsAsync();
        Task<News?> GetByIdAsync(int newsId);
        Task<News> CreateAsync(News news);
        Task<News> UpdateAsync(News news);
        Task DeleteAsync(News news);
    }
}
