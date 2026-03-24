using BlogService.Model;
using BlogService.Model.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Repository.Implementor
{
    public class NewsRepository : INewsRepository
    {
        ContentMgmtFfmContext _context;
        public NewsRepository(ContentMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<List<News>> GetNewsAsync()
        {
            return await _context.News.ToListAsync();
        }

        public async Task<News?> GetByIdAsync(int newsId)
        {
            return await _context.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
        }

        public async Task<News> CreateAsync(News news)
        {
            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task<News> UpdateAsync(News news)
        {
            _context.News.Update(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task DeleteAsync(News news)
        {
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }
    }
}
