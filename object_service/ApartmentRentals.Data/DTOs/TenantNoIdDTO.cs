namespace ApartmentRentals.Data.DTOs
{
    public class TenantNoIdDto : UserNoIdDto
    {
        public bool HasPets { get; set; }
        public bool HasChildren { get; set; }
    }
}
