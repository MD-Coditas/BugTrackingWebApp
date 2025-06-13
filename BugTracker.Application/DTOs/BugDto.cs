using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.DTOs
{
    public class BugDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; } // Use enum string (Low, Medium, High)
        public IFormFile? Screenshot { get; set; } // For file upload
        public string? ScreenshotPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ReporterId { get; set; }
        public string? Status { get; set; } // Set to "Open" by default
    }
}
