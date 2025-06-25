using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Models.Filters
{
    public class BugFilterDto
    {
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public Guid? ReporterId { get; set; }
    }
}
