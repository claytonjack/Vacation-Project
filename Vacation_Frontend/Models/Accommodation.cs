using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationBooking.Models
{
    public class Accommodation
    {
        [Key]
        public int AccommodationID { get; set; }

        [Required(ErrorMessage = "Hotel name is required")]
        [Display(Name = "Hotel Name")]
        public string HotelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; } = string.Empty;

        [Display(Name = "Accommodation Image")]
        public string? ImageUrl { get; set; }

        public virtual ICollection<Vacation> Vacations { get; set; } = new HashSet<Vacation>();
    }
} 