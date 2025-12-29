namespace TimeOfEnter.Common.Responses;

public record TokenResponse(
    string? Massage,
    bool IsAuthenticated,
    string AccessToken,
    DateTime? AccessTokenExpiresOn,
    string RefreshToken,
    DateTime? RefreshTokenExpiresOn);
