using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.WebAPI.Services.GraphQL
{
    public class Mutation
    {
        private readonly IRepository<User> _userService;

        public Mutation(IRepository<User> userService)
        {
            _userService = userService;
        }

        public async Task<string> CreateAsync(UserCreateDto dto)
        {
            User user = new User
            {
                Name = dto.Name
            };

            return await _userService.CreateAsync(user);
        }

        public async Task<bool> UpdateAsync(string id, UserDto dto) =>
             await _userService.UpdateAsync(id, new User
             {
                 Name = dto.Name,
                 registered_objects = dto.registered_objects
             });

        public async Task<bool> Delete(string id) => await _userService.DeleteByIdAsync(id);
    }
}
