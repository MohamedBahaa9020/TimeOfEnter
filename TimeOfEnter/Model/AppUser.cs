using Microsoft.AspNetCore.Identity;
using TimeOfEnter.Model;

namespace TimeOfEnter.Model
{
    public class AppUser: IdentityUser
    {
       public List<RefreshToken>? RefreshTokens {  get; set; }
    }
}
