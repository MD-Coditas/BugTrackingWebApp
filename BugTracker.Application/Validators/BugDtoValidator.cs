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
                RuleFor(x => x.Screenshot)
                    .Must(file => file == null ||
                        file.ContentType == "image/jpeg" ||
                        file.ContentType == "image/png" ||
                        file.ContentType == "image/jpg")
                    .WithMessage("Only JPG, JPEG, or PNG images are allowed.");

                RuleFor(x => x.Screenshot)
                    .Must(file => file == null ||
                        Path.GetExtension(file.FileName).ToLower() is ".jpg" or ".jpeg" or ".png")
                    .WithMessage("Only JPG, JPEG, or PNG file extensions are accepted.");

                RuleFor(x => x.Screenshot!.Length)
                    .LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Screenshot must be less than 5MB.");
            });
        }
    }
}
