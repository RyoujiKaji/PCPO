using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.Models
{
    public enum SpaceType { Apartment, Room, BedPlace, House, PartOfHouse}

    public class Space : Model
    {
        [BsonElement("category")]
        public SpaceType Category { get; set; }
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("LandlordId")]
        public string LandlordId { get; set; } = "";
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        public RegistrationStatus RegStatus { get; set; } = RegistrationStatus.NOT_REGISTRATED;
        public string AuthorId {get; set;} = string.Empty;
    }
}
