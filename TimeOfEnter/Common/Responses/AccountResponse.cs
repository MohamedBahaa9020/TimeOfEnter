using Newtonsoft.Json;

namespace TimeOfEnter.Common.Responses
{
    //public record TokenResponse(bool IsSuccess , object Token ,DateTime Expiration);
    public record TokenResponse ( 
     bool IsAuthenticated,
     string AccessToken ,
     DateTime AccessTokenExpiresOn,
     string RefreshToken, 
     DateTime RefreshTokenExpiresOn);
}
