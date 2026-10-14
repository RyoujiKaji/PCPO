using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApartmentRentals.kafka.KafkaProducer;
using System.Text.Json;
using ApartmentRentals.Exceptions;

namespace ApartmentRentals.WebAPI.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly IRepository<Tenant> _tenantService;
    private readonly IMapper _mapper;

    private readonly KafkaProducer _producer;

    public TenantController(KafkaProducer producer, IRepository<Tenant> tenantService, IMapper mapper)
    {
        _producer = producer;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<Tenant>>> GetAllAsync() => (await _tenantService.GetAllAsync()).ToList();

    [HttpGet("{id}")]
    public async Task<ActionResult<Tenant>> GetAsync(string id)
    {
        var Tenant = await _tenantService.GetByIdAsync(id);

        if (Tenant == null) return NotFound();

        return Ok(Tenant);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            var results = await _tenantService.GetFilteredByPropertyAsync(propertyName, value);
            return Ok(results);
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(TenantNoIdDto dto)
    {
        var Tenant = _mapper.Map<Tenant>(dto);

        await _tenantService.CreateAsync(Tenant);
        SendRegRequest(Tenant);

        return CreatedAtAction(nameof(GetAsync), new { id = Tenant.Id }, Tenant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, TenantNoIdDto dto) =>
        await _tenantService.UpdateAsync(id, _mapper.Map<Tenant>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _tenantService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _tenantService.DeleteAllAsync();
        return Ok();
    }

    private async void SendRegRequest(Tenant tenant) {
        RegistrationObjectDto reg = new RegistrationObjectDto()
        {
            Type = ApartmentObjectType.Tenant,
            Id = tenant.Id,
            AuthorId = tenant.AuthorId
        };

        string message = JsonSerializer.Serialize(reg);

        await _producer.SendAsync(message);
    }
}
}