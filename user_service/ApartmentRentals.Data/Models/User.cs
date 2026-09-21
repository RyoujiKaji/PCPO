using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ApartmentRentals.Data.Models
{
    public class User : IModel
    {
        public string Name { get; set; } = "";
        public int registered_objects { get; set; } = 0;
    }
}
