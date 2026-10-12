using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.Models
{
    public abstract class Model
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        required public string Id { get; set; }
    }
}
