using apbdcw8.Models.DTOs;
using apbdcw8.Services;

namespace apbdcw8.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using apbdcw8.Models.DTOs;
using apbdcw8.Services;




[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientServices _clientServices;

    public ClientsController(IClientServices clientServices)
    {
        _clientServices = clientServices;
    }

    // GET /api/clients/{id}/trips
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(string id)
    {
        if(!int.TryParse(id, out int clientId))
        {
            return BadRequest("Invalid tripId");
        }
        var clientTrips = await _clientServices.GetClientTrips(clientId);
        if (clientTrips.Count == 0)
        {
            return NotFound($"Client {{id}} not found or has no trips");
        }

        return Ok(clientTrips);
    }

    // POST /api/clients
    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
    {
        if (string.IsNullOrWhiteSpace(client.FirstName) || string.IsNullOrWhiteSpace(client.LastName) ||
            string.IsNullOrWhiteSpace(client.Pesel) || string.IsNullOrWhiteSpace(client.Email)||string.IsNullOrWhiteSpace(client.Telephone))
        {
            return BadRequest("Invalid client data");
        }
        var clientId = await _clientServices.CreateClient(client);
       
        return Created($"api/clients/{clientId}", new {id =clientId});
    }
  
    // PUT /api/clients/{id}/trips/{tripId}
    [HttpPut("{id}/trips/{tripId}")]
    public async  Task<IActionResult> RegisterClientToTrip(int id, int tripId)
    {
        var suc = await _clientServices.RegisterClientToTrip(id, tripId);
        if (!suc)
        {
            return BadRequest("Could not register client");
        }

        return Ok("Client registered to trip");
    }

    // DELETE /api/clients/{id}/trips/{tripId}
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTrip(int id, int tripId)
    {
        var suc = await _clientServices.RemoveClientFromTrip(id, tripId);
        if (!suc)
        {
            return BadRequest("Could not remove client");
        }

        return Ok("Client removed from trip");
    }
}

