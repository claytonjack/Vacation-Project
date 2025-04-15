using System;
using System.ComponentModel.DataAnnotations;

namespace VacationBooking.Models
{
    public class SearchCriteria
    {
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Maximum Price per Night")]
        public decimal MaxPricePerNight { get; set; } = 0;

        [Display(Name = "Room Type")]
        public string RoomType { get; set; } = string.Empty;

        [Display(Name = "All Inclusive")]
        public bool? AllInclusive { get; set; } = null;
    }
}