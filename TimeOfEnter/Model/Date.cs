using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TimeOfEnter.Model
{
    public class Date
    {
        public int Id { get; set; }
       
        [Required]
        public DateTime StartTime { get; set; } 

        [Required]
        public DateTime? EndTime { get; set; }
        [JsonIgnore]
        public bool IsActive { get; set; }


    }
}
