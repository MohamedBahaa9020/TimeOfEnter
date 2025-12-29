using FluentValidation;
using TimeOfEnter.DTO;
namespace TimeOfEnter.Validation;

public class DateValidator : AbstractValidator<TimeBookingWithoutIdDto>
{
    public DateValidator()
    {
        RuleFor(user => user)
            .Must(user => user.EndTime.Date > user.StartTime.Date || user.StartTime.Date == user.EndTime.Date &&
                user.EndTime.TimeOfDay > user.StartTime.TimeOfDay)
            .WithMessage("Start time must be before End time");

        RuleFor(user => user.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start time must After Now");

        RuleFor(user => user)
            .Must(user => (user.EndTime - user.StartTime).TotalMinutes >= 15)
            .WithMessage("Time between EndTime and StartTime must be more than 15 min");
    }


}