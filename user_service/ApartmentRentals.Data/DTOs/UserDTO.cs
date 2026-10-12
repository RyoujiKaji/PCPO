using ApartmentRentals.Data.Models;
using HotChocolate;

namespace ApartmentRentals.Data.DTOs
{
    public class UserDto : User
    {
        [GraphQLIgnore]
        public new string Id { get; set; } = string.Empty;
    }
}
