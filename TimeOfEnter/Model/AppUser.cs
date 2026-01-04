using Microsoft.AspNetCore.Identity;
namespace TimeOfEnter.Model;

public class AppUser : IdentityUser
{
    public string? AttachmentPath { get; set; }
    public bool IsDeleted { get; set; }
    public List<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<UserBooking> Bookings { get; set; } = [];
}
