using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VacationBooking.Models
{
    public class Vacation
    {
        [Key]
        public int VacationID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Package Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per night is required")]
        [Display(Name = "Price per Night")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "All Inclusive")]
        public bool AllInclusive { get; set; }

        [Required(ErrorMessage = "Available rooms is required")]
        [Display(Name = "Available Rooms")]
        public int AvailableRooms { get; set; }

        [Display(Name = "Vacation Image")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        public int DestinationID { get; set; }
        
        [Required(ErrorMessage = "Accommodation is required")]
        public int AccommodationID { get; set; }

        [ForeignKey("DestinationID")]
        public virtual Destination? Destination { get; set; }
        
        [ForeignKey("AccommodationID")]
        public virtual Accommodation? Accommodation { get; set; }
        
				[JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    }
}