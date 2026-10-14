using ApartmentRentals.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.DTOs
{
    public class SpaceCreateDTO
    {
        public SpaceType Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string LandlordId { get; set; } = "";
    }
}
