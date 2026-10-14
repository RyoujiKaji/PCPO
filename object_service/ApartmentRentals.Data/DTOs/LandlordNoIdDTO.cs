namespace ApartmentRentals.Data.DTOs
{
    public class LandlordNoIdDto : UserNoIdDto
    {
        public float Rating { get; set; }
        public List<string> OwnedSpaceIds { get; set; } = new();
    }
}
