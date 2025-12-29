using Microsoft.EntityFrameworkCore;
namespace TimeOfEnter.Model;

[Owned]
public class RefreshToken
{
    public required string Token { get; set; }
    public DateTime ExpireON { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpireON;
    public DateTime CreatedOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public bool IsActive => RevokedOn == null && !IsExpired;
}
