using ErrorOr;

namespace TimeOfEnter.Errors;

public static class AccountErrors
{
    public static Error InvalidCredentials => Error.Conflict(
        code: "Account.InvalidCredentials",
        description: "Invalid credentials"
    );
    public static Error EmailAlreadyInUse => Error.Conflict(
        code: "Account.EmailAlreadyInUse",
        description: "Email already in use"
    );
    public static Error UsernameAlreadyInUse => Error.Conflict(
        code: "Account.UsernameAlreadyInUse",
        description: "Username already in use"
    );
    public static Error InvalidEmailorPassword => Error.Validation(
        code: "Account.InvalidEmailFormat",
        description: "Email or Password Invalid"
    );
    public static Error WeakPassword => Error.Conflict(
        code: "Account.WeakPassword",
        description: "Password is Week must contain uppercase letters, numbers, and special characters."
    );
    public static Error RoleNotFound => Error.Conflict(
        code: "Account.RoleNotFound",
        description: "Invalid user ID or Role"
    );
    public static Error CannotAssignRole => Error.Conflict(
        code: "Account.CannotAssignRole",
        description: "User already assigned to this role"
    );
    public static Error UserNotFound => Error.NotFound(
        code: "Account.UserNotFound",
        description: "User not found"
    );
    public static Error FailedAddRole => Error.Failure(
        code: "Account.FailedAddRole",
        description: "Failed to add role"
    );
    public static Error InvalidToken => Error.Conflict(
        code: "Account.InvalidToken",
        description: "Invalid token"
    );
    public static Error ExpiredToken => Error.Failure(
        code: "Account.ExpiredToken",
        description: "Expired token"
    );
    public static Error TokenRequired => Error.Failure(
        code: "Account.TokenRequired",
        description: "Token is required"
    );
}