using ApartmentRentals.Data.Models;
using HotChocolate;

namespace ApartmentRentals.Data.DTOs
{
    public class UserCreateDto : User
    {
        [GraphQLIgnore]
        public new string Id { get; set; } = "";

        [GraphQLIgnore]
        public new int registered_objects { get; set; } = 0;
    }
}