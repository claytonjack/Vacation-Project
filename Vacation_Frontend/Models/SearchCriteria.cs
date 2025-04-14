using System;
using System.ComponentModel.DataAnnotations;

namespace VacationBooking.Models
{
    public class SearchCriteria
    {
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Maximum Price per Night")]
        public decimal MaxPricePerNight { get; set; }

        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        [Display(Name = "All Inclusive")]
        public bool? AllInclusive { get; set; }
    }
}