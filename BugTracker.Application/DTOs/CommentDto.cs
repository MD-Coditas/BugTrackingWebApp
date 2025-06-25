using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid BugId { get; set; }
        public Guid UserId { get; set; }

        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; } = string.Empty;
    }

}
