using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using HotChocolate.Authorization;


namespace ApartmentRentals.WebAPI.Services.GraphQL
{
    public class Query
    {
        private readonly MongoDbRepository<User> _userService;

        public Query(MongoDbRepository<User> userService)
        {
            _userService = userService;
        }

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<User>> GetUsersAsync() => await _userService.GetAllAsync();
    }
}
