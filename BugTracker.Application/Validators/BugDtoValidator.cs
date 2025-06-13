using BugTracker.Application.DTOs;
using FluentValidation;

namespace BugTracker.Application.Validators
{
    public class BugDtoValidator : AbstractValidator<BugDto>
    {
        public BugDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot be more than 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters long.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
                .WithMessage("Priority must be one of: Low, Medium, High.");

            When(x => x.Screenshot != null, () =>
            {
                RuleFor(x => x.Screenshot!.ContentType)
                    .Must(ct => ct == "image/jpeg" || ct == "image/png")
                    .WithMessage("Screenshot must be a JPG or PNG image.");

                RuleFor(x => x.Screenshot!.Length)
                    .LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Screenshot must be less than 5MB.");
            });
        }
    }
}
