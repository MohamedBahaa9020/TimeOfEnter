using System.ComponentModel.DataAnnotations;
namespace TimeOfEnter.DTO
{
    public class TimeOfBookingWithoutId
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
