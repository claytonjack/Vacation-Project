using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VacationBooking.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;

        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }

        public User()
        {
            Bookings = new HashSet<Booking>();
        }
    }
}