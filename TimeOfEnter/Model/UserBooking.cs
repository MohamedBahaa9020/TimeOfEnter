namespace TimeOfEnter.Model;

public class UserBooking
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int DateId { get; set; }
    public bool IsDeleted { get; set; }
    public Date Date { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
