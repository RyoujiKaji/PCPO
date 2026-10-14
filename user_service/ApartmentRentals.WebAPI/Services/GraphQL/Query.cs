using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;


namespace ApartmentRentals.WebAPI.Services.GraphQL
{
    public class Query
    {
        private readonly IRepository<User> _userService;

        public Query(IRepository<User> userService)
        {
            _userService = userService;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<User>> GetUsersAsync() => await _userService.GetAllAsync();
    }
}
