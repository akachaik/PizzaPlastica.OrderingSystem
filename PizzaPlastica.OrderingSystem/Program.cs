using PizzaPlastica.OrderingSystem.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage("table-order-storage");
    }
    
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("restaurant/{restaurantId}/tables/{tableId}", async (
    IClusterClient client,
    Guid restaurantId,
    int tableId) =>
{
    var tableOrderGrain = client.GetGrain<ITableOrderGrain>(restaurantId, tableId.ToString());

    await tableOrderGrain.OpenTableOrder();

    return Results.Ok();
})
.WithName("OpenTableOrder")
.WithOpenApi();

await app.RunAsync();

