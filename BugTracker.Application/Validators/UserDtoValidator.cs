using BugTracker.Application.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BugTracker.Application.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100)
                .Matches(@"^[A-Za-z][A-Za-z ]*$")
                .WithMessage("Username must contain letters only.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .Matches(@"^[A-Za-z0-9]*@[A-Za-z]+\.[A-Za-z]+$")
                .WithMessage("Email must be in a valid format like 'example123@domain.com'.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W_]+").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
