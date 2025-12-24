namespace TimeOfEnter.DTO;

public record TokenDto(
    string? Massage,
    bool IsAuthenticated,
    string AccessToken,
    DateTime?AccessTokenExpiresOn,
    string RefreshToken,
    DateTime?RefreshTokenExpiresOn
);