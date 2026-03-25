using InventoryService_Redis.API;
using RedisConfiguration.RedisStreamBus;
using RedisConfiguration.RedisStreamBus.Implementor;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IRedisStreamBus, RedisStreamBus>();
builder.Services.AddSingleton<InventoryStore>();
builder.Services.AddHostedService<InventoryCommandConsumer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/inventory/stocks", (InventoryStore store) => Results.Ok(store.GetStocks()));
app.MapGet("/inventory/reservations", (InventoryStore store) => Results.Ok(store.GetReservations()));

app.Run();