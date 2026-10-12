namespace ApartmentRentals.Data.DTOs
{
    public class ReceivedObjectDto{
        public ObjectType Type { get; set; }
        public string ObjectId { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}