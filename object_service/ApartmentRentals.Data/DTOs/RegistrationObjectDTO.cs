namespace ApartmentRentals.Data.DTOs
{
    public class RegistrationObjectDto{
        public ApartmentObjectType Type { get; set; }
        public string Id { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}