namespace TimeOfEnter.Model;

public class UserBooking
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int DateId { get; set; }

    // Navigation Property
    public Date Date { get; set; } = default!;
    public AppUser User { get; set; } = default!;
}
