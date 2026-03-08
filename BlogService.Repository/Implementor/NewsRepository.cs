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
    }
}
