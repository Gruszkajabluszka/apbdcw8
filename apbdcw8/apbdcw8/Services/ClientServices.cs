using apbdcw8.Models.DTOs;
using apbdcw8.Controllers;
using Microsoft.Data.SqlClient;


namespace apbdcw8.Services;

public class ClientService : IClientServices
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";


    public async Task<List<ClientTripDTO>> GetClientTrips(int id)
    {
        var trips = new List<ClientTripDTO>();
        

        string command = @"
        SELECT ct.PaymentDate, ct.RegisteredAt,
               t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
               c.IdCountry, c.Name AS CountryName
        FROM Client_Trip ct
        JOIN Trip t ON ct.IdTrip = t.IdTrip
        JOIN Country_Trip ct2 ON t.IdTrip = ct2.IdTrip
        JOIN Country c ON ct2.IdCountry = c.IdCountry
        WHERE ct.IdClient = @IdClient";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, connection))
        {
            cmd.Parameters.AddWithValue("@IdClient", id);
            await connection.OpenAsync();
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                {
                    return null;
                }
                

                while (reader.Read())
                {
                    int? paymentDate = reader["PaymentDate"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PaymentDate"]);
                    //int registeredAt = (int)reader["RegisteredAt"];
                    int idTrip = (int)reader["IdTrip"];
                    string name = (string)reader["Name"];
                    string description = (string)reader["Description"];
                    DateTime dateFrom = (DateTime)reader["DateFrom"];
                    DateTime dateTo = (DateTime)reader["DateTo"];
                    int maxPeople = (int)reader["MaxPeople"];
                    int countryId = (int)reader["IdCountry"];
                    string countryName = (string)reader["CountryName"];
                    

                    var c = new ClientTripDTO()
                    {
                        trip = new TripDTO()
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
                        },
                        paymentDate = paymentDate != null
                            ? DateTime.ParseExact(paymentDate.ToString(), "yyyyMMdd", null):
                            null
                        
                    };
                    
                    trips.Add(c);
                }
            }
        }

        return trips;
    }

    public async Task<int?> CreateClient(ClientDTO client)
    {
        string insertQ = "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) OUTPUT INSERTED.IdClient VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)";
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(insertQ, connection))
        {
            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);
            await connection.OpenAsync();
            var res = await command.ExecuteScalarAsync();
            return (int?)res;
        }
    }

    public async Task<bool> RegisterClientToTrip(int clientId, int tripId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var checkClient = new SqlCommand("SELECT Count(*) FROM Client WHERE IdClient = @Id", connection);
            checkClient.Parameters.AddWithValue("@Id", clientId);
            if ((int) await checkClient.ExecuteScalarAsync() == 0)
                return false;

            var checkTrip = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId", connection);
            checkTrip.Parameters.AddWithValue("@TripId", tripId);
            var maxPeopleObj =await checkTrip.ExecuteScalarAsync();
            if (maxPeopleObj == null)
                return false;
            int maxPeople = (int)maxPeopleObj;
            

            var countCmd = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @TripId", connection);
            countCmd.Parameters.AddWithValue("@TripId", tripId);
            int current = (int)await countCmd.ExecuteScalarAsync();
            if (current >= maxPeople)
                return false;

            var insert = new SqlCommand("INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) VALUES (@IdClient, @IdTrip, @Date, @Date)", connection);

            insert.Parameters.AddWithValue("@IdClient", clientId);
            insert.Parameters.AddWithValue("@IdTrip", tripId);
            insert.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyyMMdd"));
            

            await insert.ExecuteNonQueryAsync();
            return true;
        }
    }

    public async Task<bool> RemoveClientFromTrip(int id, int tripId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var check = new SqlCommand("SELECT 1 FROM Client_Trip WHERE IdClient = @Id AND IdTrip = @TripId",
                connection);
            check.Parameters.AddWithValue("@Id", id);
            check.Parameters.AddWithValue("@TripId", tripId);
            if (await check.ExecuteScalarAsync() == null)
                throw new KeyNotFoundException("Registration not found");

            var delete = new SqlCommand("DELETE FROM Client_Trip WHERE IdClient = @Id AND IdTrip = @TripId",
                connection);
            delete.Parameters.AddWithValue("@Id", id);
            delete.Parameters.AddWithValue("@TripId", tripId);
            await delete.ExecuteNonQueryAsync();

            return true;
        }
    }
}