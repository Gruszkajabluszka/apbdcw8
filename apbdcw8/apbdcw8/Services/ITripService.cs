using apbdcw8.Models.DTOs;

namespace apbdcw8.Services;

public interface ITripService
{
    Task<List<TripDTO>> GetTripsAsync();
   
}
