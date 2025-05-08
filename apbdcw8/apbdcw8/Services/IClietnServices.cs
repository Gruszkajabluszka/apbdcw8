using apbdcw8.Models.DTOs;


namespace apbdcw8.Services;


public interface IClientServices
{
    
    Task<List<ClientTripDTO>> GetClientTrips(int id);
    
    Task<int?> CreateClient(ClientDTO client);
    
    Task<bool> RegisterClientToTrip(int clientId, int tripId);
    
    Task<bool> RemoveClientFromTrip(int clientId, int tripId);
    
}