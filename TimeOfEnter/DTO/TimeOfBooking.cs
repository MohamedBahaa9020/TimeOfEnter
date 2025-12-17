using System.ComponentModel.DataAnnotations;

namespace TimeOfEnter.DTO
{
    public class TimeOfBooking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
