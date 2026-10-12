using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using ApartmentRentals.WebAPI.Services.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApartmentRentals.kafka.KafkaProducer;
using System.Text.Json;
using System.Threading.Tasks;
using ApartmentRentals.Exceptions;

namespace ApartmentRentals.WebAPI.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class RentalContractController : ControllerBase
{
    private readonly IRepository<RentalContract> _rentalService;
    private readonly IMapper _mapper;

    private readonly KafkaProducer _producer;

    public RentalContractController(KafkaProducer producer, IRepository<RentalContract> rentalService, IMapper mapper)
    {
        _producer = producer;
        _rentalService = rentalService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<RentalContract>>> GetAllAsync() => (await _rentalService.GetAllAsync()).ToList();

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalContract>> GetAsync(string id)
    {
        var rentalContract = await _rentalService.GetByIdAsync(id);

        if (rentalContract == null) return NotFound();

        return Ok(rentalContract);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredByPropertyAsync([FromQuery] string propertyName, [FromQuery] string value)
    {
        try
        {
            return Ok(await _rentalService.GetFilteredByPropertyAsync(propertyName, value));
        }
        catch (PropertyFilterException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(RentalContractNoIdDto dto)
    {
        var rentalContract = _mapper.Map<RentalContract>(dto);

        await _rentalService.CreateAsync(rentalContract);
        await SendRegRequest(rentalContract);

        return CreatedAtAction(nameof(GetAsync), new { id = rentalContract.Id }, rentalContract);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, RentalContractNoIdDto dto) => 
        await _rentalService.UpdateAsync(id, _mapper.Map<RentalContract>(dto)) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await _rentalService.DeleteByIdAsync(id) ? NoContent() : NotFound();

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        await _rentalService.DeleteAllAsync();
        return Ok();
    }

    private async Task SendRegRequest(RentalContract rentalContract) {
        RegistrationObjectDto reg = new RegistrationObjectDto()
        {
            Type = ObjectType.RentalContract,
            Id = rentalContract.Id,
            AuthorId = rentalContract.AuthorId
        };

        string message = JsonSerializer.Serialize(reg);

        await _producer.SendAsync(message);
    }
}
}