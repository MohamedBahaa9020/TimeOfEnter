using FluentValidation;
using TimeOfEnter.DTO;

namespace TimeOfEnter.Validation
{
    public class AddUserRole:AbstractValidator<AddRole>
    {
        public AddUserRole()
        {
            RuleFor (x => x.UserId).NotEmpty()
                .WithMessage("UserId is Required");
            RuleFor(x => x.UserId).NotEmpty()
                .WithMessage("Role is Required");


        }
    }
}
