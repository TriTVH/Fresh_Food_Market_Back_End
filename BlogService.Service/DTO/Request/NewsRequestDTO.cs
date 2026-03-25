using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Service.DTO.Request
{
    public class NewsRequestDTO
    {
        public int? SubCategoryId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
