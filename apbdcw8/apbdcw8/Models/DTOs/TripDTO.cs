namespace apbdcw8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string Description  { get; set; }
    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }
    
    public CountryDTO Country { get; set; }
}

public class CountryDTO
{
    public int CountryId { get; set; }
    public string CountryName { get; set; }
    
}

public class ClientTripDTO
{
    public TripDTO trip { get; set; }
    public DateTime? paymentDate { get; set; }
    
    
}