using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace BugTracker.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository repo, IUserRepository userRepo, ILogger<CommentService> logger)
        {
            _repo = repo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task AddCommentAsync(CommentDto dto)
        {
            try
            {
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    BugId = dto.BugId,
                    UserId = dto.UserId,
                    Message = dto.Message,
                    CreatedAt = DateTime.Now
                };

                await _repo.AddAsync(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to bug ID {BugId} by user ID {UserId}", dto.BugId, dto.UserId);
                throw new ApplicationException("Failed to add comment.", ex);
            }

        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByBugIdAsync(Guid bugId)
        {
            try
            {
                var comments = await _repo.GetByBugIdAsync(bugId);

                return comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    BugId = c.BugId,
                    UserId = c.UserId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User.UserName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comments for bug ID {BugId}", bugId);
                throw new ApplicationException("Failed to retrieve comments.", ex);
            }

        }
    }
}
