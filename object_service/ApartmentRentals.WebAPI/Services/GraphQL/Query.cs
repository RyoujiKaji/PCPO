using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using HotChocolate.Data;

namespace ApartmentRentals.WebAPI.Services.GraphQL
{
    public class Query
    {
        private readonly IRepository<Space> _spaces;
        private readonly IRepository<Landlord> _landlords;
        private readonly IRepository<Tenant> _tenants;
        private readonly IRepository<RentalContract> _contracts;

        public Query(IRepository<Space> spaces, IRepository<Landlord> landlords,
            IRepository<Tenant> tenants, IRepository<RentalContract> contracts)
        {
            _contracts = contracts;
            _spaces = spaces;  
            _landlords = landlords;
            _tenants = tenants;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Space>> GetSpacesAsync() => await _spaces.GetAllAsync();

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Landlord>> GetLandlordsAsync() => await _landlords.GetAllAsync();

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<Tenant>> GetTenantsAsync() => await _tenants.GetAllAsync();

        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<RentalContract>> GetRentalContractsAsync() => await _contracts.GetAllAsync();
    }
}
