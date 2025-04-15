using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationBooking.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int VacationID { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Number of nights is required")]
        [Display(Name = "Number of Nights")]
        [Range(1, 30, ErrorMessage = "Number of nights must be between 1 and 30")]
        public int NumberOfNights { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Display(Name = "Number of Guests")]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        public int NumberOfGuests { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        
        [ForeignKey("VacationID")]
        public virtual Vacation Vacation { get; set; }
    }
}