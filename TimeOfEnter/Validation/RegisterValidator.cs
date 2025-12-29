using FluentValidation;
using TimeOfEnter.DTO;
namespace TimeOfEnter.Validation;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
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
