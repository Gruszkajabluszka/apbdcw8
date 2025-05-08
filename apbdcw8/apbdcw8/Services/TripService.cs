using apbdcw8.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;



namespace apbdcw8.Services;


public class TripService : ITripService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";



    public async Task<List<TripDTO>> GetTripsAsync()
    {
        var trips = new List<TripDTO>();
        var command = @"
    SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
           c.IdCountry, c.Name AS CountryName
    FROM Trip t
    JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
    JOIN Country c ON ct.IdCountry = c.IdCountry";

        using (var connection = new SqlConnection(_connectionString))
        using (var com = new SqlCommand(command, connection))
        {

            await connection.OpenAsync();

            using (var reader = await com.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idTrip = (int)reader["IdTrip"];
                    string name = (string)reader["Name"];
                    string description = (string)reader["Description"];
                    DateTime dateFrom = (DateTime)reader["DateFrom"];
                    DateTime dateTo = (DateTime)reader["DateTo"];
                    int maxPeople = (int)reader["MaxPeople"];
                    int countryId = (int)reader["IdCountry"];
                    string countryName = (string)reader["CountryName"];

                    var s = new TripDTO()
                    {
                        Id = idTrip,
                        Name = name,
                        Description = description,
                        DateFrom = dateFrom,
                        DateTo = dateTo,
                        MaxPeople = maxPeople,
                        Country = new CountryDTO()
                        {
                            CountryId = countryId,
                            CountryName = countryName
                        }

                    };
                    trips.Add(s);
                }
            }
        }

        return trips;
    }
}