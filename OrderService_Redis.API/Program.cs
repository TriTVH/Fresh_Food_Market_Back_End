using OrderService_Redis.API;
using RedisConfiguration.Command;
using RedisConfiguration.Contract;
using RedisConfiguration.Message;
using RedisConfiguration.RedisStreamBus;
using RedisConfiguration.RedisStreamBus.Implementor;
using RedisConfiguration.Utils;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IRedisStreamBus, RedisStreamBus>();
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddSingleton<SagaStore>();
builder.Services.AddHostedService<SagaReplyConsumer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/orders", (CreateOrderRequest request, OrderStore orderStore) =>
{
    if (string.IsNullOrWhiteSpace(request.UserId))
        return Results.BadRequest(new { message = "UserId is required" });

    if (request.Items is null || request.Items.Count == 0)
        return Results.BadRequest(new { message = "Items are required" });

    var order = orderStore.Create(request);

    return Results.Ok(new
    {
        order.OrderId,
        order.UserId,
        order.PaymentMethod,
        order.PaymentStatus,
        order.OrderStatus
    });
});

app.MapGet("/orders", (OrderStore orderStore) => Results.Ok(orderStore.GetAll()));

app.MapGet("/orders/{orderId}", (string orderId, OrderStore orderStore) =>
{
    var order = orderStore.Get(orderId);
    return order is null ? Results.NotFound() : Results.Ok(order);
});

app.MapGet("/sagas", (SagaStore sagaStore) => Results.Ok(sagaStore.GetAll()));

app.MapPost("/orders/{orderId}/move-to-packaging", async (
    string orderId,
    OrderStore orderStore,
    SagaStore sagaStore,
    IRedisStreamBus bus) =>
{
    var order = orderStore.Get(orderId);
    if (order is null)
        return Results.NotFound(new { message = "Order not found" });

    if (!orderStore.CanMoveToPackaging(order))
    {
        return Results.BadRequest(new
        {
            message = "Order is not eligible to move to PACKAGING",
            order.OrderStatus,
            order.PaymentMethod,
            order.PaymentStatus
        });
    }

    order.OrderStatus = "RESERVING_INVENTORY";

    var saga = sagaStore.CreateFromOrder(order);

    var command = new ReserveInventoryCommand
    {
        SagaId = saga.SagaId,
        OrderId = order.OrderId,
        Items = order.Items
    };

    await bus.PublishAsync(StreamConstants.InventoryCommands, new StreamEnvelope
    {
        MessageType = MessageType.ReserveInventory,
        Source = "order-service",
        SagaId = saga.SagaId,
        CorrelationId = order.OrderId,
        Payload = JsonHelper.Serialize(command)
    });

    return Results.Ok(new
    {
        message = "Reserve inventory started",
        sagaId = saga.SagaId,
        orderId = order.OrderId,
        orderStatus = order.OrderStatus
    });
});

app.Run();