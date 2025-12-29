namespace TimeOfEnter.Model;
public class UserIsBooking
{
    public int Id { get; set; }
    public  string? UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

}
