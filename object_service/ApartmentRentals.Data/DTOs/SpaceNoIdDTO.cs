using ApartmentRentals.Data.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentRentals.Data.DTOs
{
    public class SpaceNoIdDTO
    {
        public SpaceType Category { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string LandlordId { get; set; } = "";
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        public RegistrationStatus RegStatus { get; set; } = RegistrationStatus.NOT_REGISTRATED;
        public string AuthorId { get; set; } = string.Empty;
    }
}
