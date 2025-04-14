using System.ComponentModel.DataAnnotations;

namespace VacationBooking.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool Remember { get; set; }

        public string ReturnUrl { get; set; }
    }
} 