using ErrorOr;

namespace TimeOfEnter.Errors;

public class DateErrors
{
    public static Error TimeAlreadyBooking => Error.Conflict(
        code: "Date.TimeAlreadyBooking",
        description: "This Time is Already Booking"
    );
    public static Error NoDatesAvailable => Error.NotFound(
        code: "Date.NoDatesAvailable",
        description: "No Dates Available"
    );
    public static Error InvalidDate => Error.Conflict(
        code: "Date.InvalidDate",
        description: "No Dates Found."
    );
    public static Error UserRequired => Error.NotFound(
        code: "User.Required",
        description: "User Is Required."
    );
    public static Error NoBookingsFound => Error.NotFound(
        code: "Date.BookingsNotFound",
        description: "NoBookingsFound."
    );
}
