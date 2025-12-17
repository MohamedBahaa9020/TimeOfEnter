using System.ComponentModel.DataAnnotations;

namespace TimeOfEnter.Model
{
    public class Date
    {
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; } 

        [Required]
        public DateTime? EndTime { get; set; }


    }
}
