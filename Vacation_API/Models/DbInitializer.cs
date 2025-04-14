using System;
using System.Linq;
using VacationBooking.Models;
using Microsoft.AspNetCore.Identity;

namespace VacationBooking.Data
{
    public static class DbInitializer
    {
        public static void Initialize(VacationDbContext context, UserManager<User> userManager)
        {
            if (context.Vacations.Any())
            {
                return;
            }

            var destinations = new Destination[]
            {
                new Destination
                {
                    City = "London",
                    Country = "England",
                    ImageUrl = "/images/destination/london-destination.png"
                },
                new Destination
                {
                    City = "Paris",
                    Country = "France",
                    ImageUrl = "/images/destination/paris-destination.png"
                },
                new Destination
                {
                    City = "Barcelona",
                    Country = "Spain",
                    ImageUrl = "/images/destination/barcelona-destination.png"
                },
                new Destination
                {
                    City = "Prague",
                    Country = "Czech Republic",
                    ImageUrl = "/images/destination/prague-destination.png"
                },
                new Destination
                {
                    City = "Brussels",
                    Country = "Belgium",
                    ImageUrl = "/images/destination/brussels-destination.png"
                },
                new Destination
                {
                    City = "Edinburgh",
                    Country = "Scotland",
                    ImageUrl = "/images/destination/edinburgh-destination.png"
                },
                new Destination
                {
                    City = "Berlin",
                    Country = "Germany",
                    ImageUrl = "/images/destination/berlin-destination.png"
                },
                new Destination
                {
                    City = "Seoul",
                    Country = "South Korea",
                    ImageUrl = "/images/destination/seoul-destination.png"
                },
                new Destination
                {
                    City = "Tokyo",
                    Country = "Japan",
                    ImageUrl = "/images/destination/tokyo-destination.png"
                },
                new Destination
                {
                    City = "Mexico City",
                    Country = "Mexico",
                    ImageUrl = "/images/destination/mexico-destination.png"
                }
            };

            context.Destinations.AddRange(destinations);
            context.SaveChanges();

            var accommodations = new Accommodation[]
            {
                new Accommodation
                {
                    HotelName = "The Grand Hotel",
                    Address = "1 Main St, London",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/london-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Paris Luxury Suites",
                    Address = "15 Rue de Rivoli, Paris",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/paris-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Barcelona Beach Resort",
                    Address = "35 La Rambla, Barcelona",
                    RoomType = "Resort",
                    ImageUrl = "/images/accommodation/barcelona-resort.png"
                },
                new Accommodation
                {
                    HotelName = "Prague Castle View",
                    Address = "22 Old Town Square, Prague",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/prague-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Brussels Central Stay",
                    Address = "5 Grand Place, Brussels",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/brussels-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Edinburgh Royal Residence",
                    Address = "28 Royal Mile, Edinburgh",
                    RoomType = "Resort",
                    ImageUrl = "/images/accommodation/edinburgh-resort.png"
                },
                new Accommodation
                {
                    HotelName = "Berlin Modern Apartments",
                    Address = "44 Unter den Linden, Berlin",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/berlin-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "London Riverside Inn",
                    Address = "42 Thames St, London",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/london-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Paris Eiffel View",
                    Address = "78 Avenue Kléber, Paris",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/paris-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Barcelona City Center",
                    Address = "112 Carrer de Balmes, Barcelona",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/barcelona-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Prague Old Town Apartments",
                    Address = "15 Wenceslas Square, Prague",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/prague-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Brussels Boutique Hotel",
                    Address = "29 Avenue Louise, Brussels",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/brussels-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Edinburgh Castle Hostel",
                    Address = "14 Grassmarket, Edinburgh",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/edinburgh-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Berlin Historic Hotel",
                    Address = "33 Friedrichstrasse, Berlin",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/berlin-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Seoul Plaza Hotel",
                    Address = "119 Sogong-ro, Jung-gu, Seoul",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/seoul-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Gangnam Residence",
                    Address = "45 Teheran-ro, Gangnam-gu, Seoul",
                    RoomType = "Resort",
                    ImageUrl = "/images/accommodation/seoul-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Shinjuku Sky Tower",
                    Address = "3-5-8 Shinjuku, Tokyo",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/tokyo-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Asakusa Traditional Inn",
                    Address = "2-1-5 Asakusa, Taito-ku, Tokyo",
                    RoomType = "Hostel",
                    ImageUrl = "/images/accommodation/tokyo-hostel.png"
                },
                new Accommodation
                {
                    HotelName = "Mexico City Grand Palace",
                    Address = "125 Paseo de la Reforma, Mexico City",
                    RoomType = "Hotel",
                    ImageUrl = "/images/accommodation/mexico-hotel.png"
                },
                new Accommodation
                {
                    HotelName = "Zocalo Historic Suites",
                    Address = "58 Av. 5 de Mayo, Centro Histórico, Mexico City",
                    RoomType = "Resort",
                    ImageUrl = "/images/accommodation/mexico-resort.png"
                }
            };

            context.Accommodations.AddRange(accommodations);
            context.SaveChanges();

            var vacations = new Vacation[]
            {
                new Vacation
                {
                    Name = "London City Break",
                    Description = "Experience the historic charm and modern excitement of London...",
                    PricePerNight = 289.99m,
                    AllInclusive = false,
                    AvailableRooms = 12,
                    ImageUrl = "/images/vacation/london-vacation.png",
                    DestinationID = destinations[0].DestinationID,
                    AccommodationID = accommodations[0].AccommodationID
                },
                new Vacation
                {
                    Name = "Paris Romantic Escape",
                    Description = "Immerse yourself in the charm and romance of Paris...",
                    PricePerNight = 249.99m,
                    AllInclusive = true,
                    AvailableRooms = 15,
                    ImageUrl = "/images/vacation/paris-vacation.png",
                    DestinationID = destinations[1].DestinationID,
                    AccommodationID = accommodations[1].AccommodationID
                },
                new Vacation
                {
                    Name = "Barcelona Beach Adventure",
                    Description = "Relax on the beautiful beaches and explore vibrant culture...",
                    PricePerNight = 219.99m,
                    AllInclusive = false,
                    AvailableRooms = 10,
                    ImageUrl = "/images/vacation/barcelona-vacation.png",
                    DestinationID = destinations[2].DestinationID,
                    AccommodationID = accommodations[2].AccommodationID
                },
                new Vacation
                {
                    Name = "Prague Fairytale Getaway",
                    Description = "Step into a fairytale as you explore the historic streets of Prague...",
                    PricePerNight = 199.99m,
                    AllInclusive = false,
                    AvailableRooms = 8,
                    ImageUrl = "/images/vacation/prague-vacation.png",
                    DestinationID = destinations[3].DestinationID,
                    AccommodationID = accommodations[3].AccommodationID
                },
                new Vacation
                {
                    Name = "Brussels Cultural Journey",
                    Description = "Experience the rich culture, history, and chocolate delights of Brussels...",
                    PricePerNight = 229.99m,
                    AllInclusive = false,
                    AvailableRooms = 12,
                    ImageUrl = "/images/vacation/brussels-vacation.png",
                    DestinationID = destinations[4].DestinationID,
                    AccommodationID = accommodations[4].AccommodationID
                },
                new Vacation
                {
                    Name = "Edinburgh Highlands Adventure",
                    Description = "Explore the stunning landscapes and historical sites of Scotland...",
                    PricePerNight = 239.99m,
                    AllInclusive = false,
                    AvailableRooms = 10,
                    ImageUrl = "/images/vacation/edinburgh-vacation.png",
                    DestinationID = destinations[5].DestinationID,
                    AccommodationID = accommodations[5].AccommodationID
                },
                new Vacation
                {
                    Name = "Berlin Historic Exploration",
                    Description = "Discover the rich history and modern culture of Berlin...",
                    PricePerNight = 259.99m,
                    AllInclusive = false,
                    AvailableRooms = 9,
                    ImageUrl = "/images/vacation/berlin-vacation.png",
                    DestinationID = destinations[6].DestinationID,
                    AccommodationID = accommodations[6].AccommodationID
                },
            };

            context.Vacations.AddRange(vacations);
            context.SaveChanges();

            if (!userManager.Users.Any())
            {
                var regularUser = new User
                {
                    UserName = "bob@gmail.com",
                    Email = "bob@gmail.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    PhoneNumber = "905-845-4767",
                    Address = "412 Bath Ave, New York, USA",
                    IsAdmin = false,
                    EmailConfirmed = true
                };
                
                var result = userManager.CreateAsync(regularUser, "Hello123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(regularUser, "User").Wait();
                }

                var adminUser = new User
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Account",
                    PhoneNumber = "905-639-5646",
                    Address = "1256 Kelway St, Dallas, USA",
                    IsAdmin = true,
                    EmailConfirmed = true
                };
                
                result = userManager.CreateAsync(adminUser, "Hello123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                }
            }
        }
    }
}