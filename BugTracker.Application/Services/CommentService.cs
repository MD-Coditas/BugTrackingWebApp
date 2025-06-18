using BugTracker.Application.DTOs;
using BugTracker.Application.Interfaces;
using BugTracker.Infrastructure.Interfaces;
using BugTracker.Infrastructure.Models;
using BugTracker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;
        private readonly IUserRepository _userRepo;

        public CommentService(ICommentRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task AddCommentAsync(CommentDto dto)
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

        public async Task<IEnumerable<CommentDto>> GetCommentsByBugIdAsync(Guid bugId)
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
    }

}
