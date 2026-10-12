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
public class LandlordController : ControllerBase
{
    private readonly IRepository<Landlord> _landlordService;
    private readonly IMapper _mapper;
    private readonly KafkaProducer _producer;

    public LandlordController(KafkaProducer producer, IRepository<Landlord> landlordService, IMapper mapper)
    {
        _producer = producer;
        _landlordService = landlordService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<Landlord>>> GetAllAsync() => (await _landlordService.GetAllAsync()).ToList();

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            return Ok(await _landlordService.GetFilteredByPropertyAsync(propertyName, value));
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Landlord>> GetAsync(string id)
    {
        var landlord = await _landlordService.GetByIdAsync(id);

        if (landlord == null) return NotFound();

        return Ok(landlord);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(LandlordNoIdDto dto)
    {
        var landlord = _mapper.Map<Landlord>(dto);
        await _landlordService.CreateAsync(landlord);
        SendRegRequest(landlord);

        return CreatedAtAction(nameof(GetAsync), new { id = landlord.Id }, landlord);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, LandlordNoIdDto dto) => 
        await _landlordService.UpdateAsync(id, _mapper.Map<Landlord>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _landlordService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _landlordService.DeleteAllAsync();
        return Ok();
    }
    private async void SendRegRequest(Landlord l) {
        RegistrationObjectDto reg = new RegistrationObjectDto()
        {
            Type = ObjectType.Landlord,
            Id = l.Id,
            AuthorId = l.AuthorId
        };

        string message = JsonSerializer.Serialize(reg);

        await _producer.SendAsync(message);
    }
}
}
