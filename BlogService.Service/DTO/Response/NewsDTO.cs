using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Service.DTO.Response
{
    public class NewsDTO
    {
        public int NewsId { get; set; }

        public int? StaffId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Image { get; set; }

        public string? Category { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
