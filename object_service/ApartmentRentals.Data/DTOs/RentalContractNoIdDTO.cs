using ApartmentRentals.Data.Models;

namespace ApartmentRentals.Data.DTOs
{
    public class RentalContractNoIdDTO
    {
        public string SpaceId { get; set; } = "";
        public string TenantId { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string AuthorId {get; set;} = "string";
        public RegistrationStatus RegStatus {get; init;}
    }
}
