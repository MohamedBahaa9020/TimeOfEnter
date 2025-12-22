using Newtonsoft.Json;

namespace TimeOfEnter.DTO
{
    public class TokenDto
    {
           public string? Massage {  get; set; }
            public bool IsAuthenticated { get; set; }
            public string AccessToken { get; set; }
            public DateTime AccessTokenExpiresOn { get; set; }
            public string RefreshToken { get; set; }
            public DateTime RefreshTokenExpiresOn { get; set; }     
 
    }
}
