using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.DTOs
{
    public class PagedResult<T> : PagedResult
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}

