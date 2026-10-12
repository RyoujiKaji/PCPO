namespace ApartmentRentals.Data.DTOs
{
    public class LandlordNoIdDto : UserNoIdDto
    {
        public float Rating { get; set; }
        public List<int> OwnedSpaceIds { get; set; } = new();
    }
}
