using apbdcw8.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IClientServices, ClientService>();


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();