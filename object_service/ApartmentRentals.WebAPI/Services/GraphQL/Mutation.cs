using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.WebAPI.Services.GraphQL
{
    public class Mutation
    {
        private readonly IRepository<Space> _spaces;
        private readonly IRepository<Landlord> _landlords;
        private readonly IRepository<Tenant> _tenants;
        private readonly IRepository<RentalContract> _contracts;

        public Mutation(IRepository<Space> spaces, IRepository<Landlord> landlords,
            IRepository<Tenant> tenants, IRepository<RentalContract> contracts)
        {
            _contracts = contracts;
            _spaces = spaces;
            _landlords = landlords;
            _tenants = tenants;
        }

        // =========== Space ================
        public async Task<string> CreateSpaceAsync(SpaceCreateDTO dto)
        {
            Space obj = new Space
            {
                Category = dto.Category,
                Title = dto.Title,
                Address = dto.Address,
                Price = dto.Price,
                LandlordId = dto.LandlordId
            };

            return await _spaces.CreateAsync(obj);
        }

        public async Task<bool> UpdateSpaceAsync(string id, SpaceNoIdDTO dto) =>
             await _spaces.UpdateAsync(id, new Space
             {
                 Category = dto.Category,
                 Title = dto.Title,
                 Address = dto.Address,
                 Price = dto.Price,
                 LandlordId = dto.LandlordId,
                 PublishedAt = dto.PublishedAt,
                 RegStatus = dto.RegStatus,
                 AuthorId = dto.AuthorId
             });

        public async Task<bool> DeleteSpaceAsync(string id) => await _spaces.DeleteByIdAsync(id);

        // =========== Landlord ================
        public async Task<string> CreateLandlordAsync(LandlordNoIdDTO dto)
        {
            Landlord obj = new Landlord
            {
                Surname = dto.Surname,
                Name = dto.Name,
                MiddleName = dto.MiddleName,
                Phone = dto.Phone,
                OwnedSpaceIds = dto.OwnedSpaceIds,
                Rating = dto.Rating,
                AuthorId = dto.AuthorId,
                RegStatus = dto.RegStatus
            };

            return await _landlords.CreateAsync(obj);
        }

        public async Task<bool> UpdateLandlordAsync(string id, LandlordNoIdDTO dto) =>
             await _landlords.UpdateAsync(id, new Landlord
             {
                 Surname = dto.Surname,
                 Name = dto.Name,
                 MiddleName = dto.MiddleName,
                 Phone = dto.Phone,
                 OwnedSpaceIds = dto.OwnedSpaceIds,
                 Rating = dto.Rating,
                 AuthorId = dto.AuthorId,
                 RegStatus = dto.RegStatus
             });

        public async Task<bool> DeleteLandlordAsync(string id) => await _landlords.DeleteByIdAsync(id);

        // =========== Tenant ================
        public async Task<string> CreateTenantAsync(TenantNoIdDTO dto)
        {
            Tenant obj = new Tenant
            {
                Surname = dto.Surname,
                Name = dto.Name,
                MiddleName = dto.MiddleName,
                Phone = dto.Phone,
                AuthorId = dto.AuthorId,
                RegStatus = dto.RegStatus,
                HasChildren = dto.HasChildren,
                HasPets = dto.HasPets
            };

            return await _tenants.CreateAsync(obj);
        }

        public async Task<bool> UpdateTenantAsync(string id, TenantNoIdDTO dto) =>
             await _tenants.UpdateAsync(id, new Tenant
             {
                 Surname = dto.Surname,
                 Name = dto.Name,
                 MiddleName = dto.MiddleName,
                 Phone = dto.Phone,
                 AuthorId = dto.AuthorId,
                 RegStatus = dto.RegStatus,
                 HasChildren = dto.HasChildren,
                 HasPets = dto.HasPets
             });

        public async Task<bool> DeleteTenantAsync(string id) => await _tenants.DeleteByIdAsync(id);

        // =========== RentalContract ================
        public async Task<string> CreateRentalContractAsync(RentalContractNoIdDTO dto)
        {
            RentalContract obj = new RentalContract
            {
                TenantId = dto.TenantId,
                SpaceId = dto.SpaceId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AuthorId = dto.AuthorId,
                RegStatus = dto.RegStatus
            };

            return await _contracts.CreateAsync(obj);
        }

        public async Task<bool> UpdateRentalContractAsync(string id, RentalContractNoIdDTO dto) =>
             await _contracts.UpdateAsync(id, new RentalContract
             {
                 TenantId = dto.TenantId,
                 SpaceId = dto.SpaceId,
                 StartDate = dto.StartDate,
                 EndDate = dto.EndDate,
                 AuthorId = dto.AuthorId,
                 RegStatus = dto.RegStatus
             });

        public async Task<bool> DeleteRentalContractAsync(string id) => await _contracts.DeleteByIdAsync(id);
    }
}
