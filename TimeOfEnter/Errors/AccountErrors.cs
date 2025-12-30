using ErrorOr;

namespace TimeOfEnter.Errors;

public static class AccountErrors
{
    public static Error InvalidCredentials => Error.Validation(
        code: "Account.InvalidCredentials",
        description: "Invalid credentials"
    );
    public static Error EmailAlreadyInUse => Error.Validation(
        code: "Account.EmailAlreadyInUse",
        description: "Email already in use"
    );
    public static Error UsernameAlreadyInUse => Error.Validation(
        code: "Account.UsernameAlreadyInUse",
        description: "Username already in use"
    );
    public static Error InvalidToken => Error.Validation(
        code: "Account.InvalidToken",
        description: "Invalid token"
    );
}