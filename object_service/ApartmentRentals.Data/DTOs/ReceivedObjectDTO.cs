namespace ApartmentRentals.Data.DTOs
{
    public class ReceivedObjectDTO{
        public ApartmentObjectType Type { get; set; }
        public string ObjectId { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}