using Microsoft.AspNetCore.Identity;
namespace TimeOfEnter.Model;

public class AppUser : IdentityUser
{
    public List<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<UserBooking> Bookings { get; set; } = [];
}
