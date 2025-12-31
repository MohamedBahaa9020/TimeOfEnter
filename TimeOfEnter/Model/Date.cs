namespace TimeOfEnter.Model;

public class Date
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive { get; set; }
    public ICollection<UserBooking> Bookings { get; set; } = [];
}
