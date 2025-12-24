using FluentValidation;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Validation
{
    public class RegisterValid : AbstractValidator<RegisterDto>
    {
        public RegisterValid()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required");

            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password is required");
        }
    }
}
