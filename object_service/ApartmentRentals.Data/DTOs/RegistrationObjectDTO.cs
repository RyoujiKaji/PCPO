namespace ApartmentRentals.Data.DTOs
{
    public class RegistrationObjectDTO{
        public ApartmentObjectType Type { get; set; }
        public string Id { get; set; } = "";
        public string AuthorId { get; set; } = "";
    }
}