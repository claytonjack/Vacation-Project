using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VacationBooking.Models
{
    public class Destination
    {
        [Key]
        public int DestinationID { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Destination Image")]
        public string? ImageUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<Vacation> Vacations { get; set; } = new HashSet<Vacation>();
    }
} 