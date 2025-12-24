using FluentValidation;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Validation;
public class AddUserRoleValidator : AbstractValidator<AddRole>
{
    public AddUserRoleValidator()
    {
        RuleFor(x => x.UserId).NotEmpty()
            .WithMessage("UserId is Required");
        RuleFor(x => x.UserId).NotEmpty()
            .WithMessage("Role is Required");


    }

}
