namespace ApartmentRentals.Data.DTOs
{
    public class LandlordNoIdDTO : UserNoIdDTO
    {
        public float Rating { get; set; }
        public List<string> OwnedSpaceIds { get; set; } = new();
    }
}
