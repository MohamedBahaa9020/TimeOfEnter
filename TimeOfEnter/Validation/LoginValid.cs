using FluentValidation;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Validation
{
    public class LoginValid : AbstractValidator<LoginDto>
    {
        public LoginValid()
        {
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password is required");
        }
    }
}
