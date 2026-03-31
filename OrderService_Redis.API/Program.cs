using OrderService_Redis.API;
using OrderService_Redis.API.Entities;
using RedisConfiguration.Command;
using RedisConfiguration.Contract;
using RedisConfiguration.DTOs;
using RedisConfiguration.Message;
using RedisConfiguration.RedisStreamBus;
using RedisConfiguration.RedisStreamBus.Implementor;
using RedisConfiguration.Utils;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddSingleton<IRedisStreamBus, RedisStreamBus>();
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddSingleton<SagaStore>();
builder.Services.AddHostedService<SagaReplyConsumer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new System.Collections.Generic.List<string>()
        }
    });
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/orders", async (CreateOrderRequest request, OrderStore orderStore) =>
{

    if (request.Items is null || request.Items.Count == 0)
        return Results.BadRequest(new { message = "Items are required" });

    var order = await orderStore.Create(request);

    return Results.Ok(new
    {
        order.OrderId,
        order.OrderStatus
    });
});

app.MapGet("/orders", async (OrderStore orderStore) =>
{
    var orders = await orderStore.GetAll();
    return Results.Ok(orders);
});

app.MapGet("/orders/{orderId}", async (string orderId, OrderStore orderStore) =>
{
    var order = await orderStore.Get(orderId);
    return order is null ? Results.NotFound() : Results.Ok(order);
});

app.MapGet("/sagas", (SagaStore sagaStore) => Results.Ok(sagaStore.GetAll()));

app.MapPost("/orders/{orderId}/move-to-packaging", async (
    string orderId,
    OrderStore orderStore,
    SagaStore sagaStore,
    IRedisStreamBus bus) =>
{
    var order = await orderStore.Get(orderId);
    if (order is null)
        return Results.NotFound(new { message = "Order not found" });

    if (!orderStore.CanMoveToPackaging(order))
    {
        return Results.BadRequest(new
        {
            message = "Order is not eligible to move to PACKAGING",
            order.OrderStatus,
        });
    }

    

    var saga = sagaStore.CreateFromOrder(order);

    var command = new ReserveInventoryCommand
    {
        SagaId = saga.SagaId,
        OrderId = order.OrderId,
        Items = order.Items.Select(i => new OrderItemDTO
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            CreatedAt = DateTime.UtcNow // hoặc i.CreatedAt nếu có
        }).ToList()
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

// ── VNPAY Payment endpoints ──────────────────────────────────────────────────

app.MapPost("/payment/vnpay/create", async (
    VnpayPayRequest req,
    OrderStore orderStore,
    IConfiguration config) =>
{
    var order = await orderStore.Get(req.OrderNumber);
    if (order is null)
        return Results.NotFound(new { success = false, message = $"Order '{req.OrderNumber}' not found" });

    if (order.OrderStatus != "PENDING")
        return Results.BadRequest(new { success = false, message = $"Order is not eligible for payment (status: {order.OrderStatus})" });

    var tmnCode   = config["Vnpay:TmnCode"]!;
    var hashSecret = config["Vnpay:HashSecret"]!;
    var baseUrl   = config["Vnpay:BaseUrl"]!;
    var returnUrl = config["Vnpay:ReturnUrl"]!;
    var version   = config["Vnpay:Version"]!;
    var command   = config["Vnpay:Command"]!;
    var currCode  = config["Vnpay:CurrCode"]!;
    var locale    = config["Vnpay:Locale"]!;

    var amount = (long)(order.TotalAmount * 100);
    var createDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
    var ipAddr = "127.0.0.1";

    var vnpParams = new SortedDictionary<string, string>
    {
        ["vnp_Version"]    = version,
        ["vnp_Command"]    = command,
        ["vnp_TmnCode"]    = tmnCode,
        ["vnp_Amount"]     = amount.ToString(),
        ["vnp_CurrCode"]   = currCode,
        ["vnp_TxnRef"]     = order.OrderId,
        ["vnp_OrderInfo"]  = $"Thanh toan don hang {order.OrderId}",
        ["vnp_OrderType"]  = "other",
        ["vnp_Locale"]     = locale,
        ["vnp_ReturnUrl"]  = returnUrl,
        ["vnp_IpAddr"]     = ipAddr,
        ["vnp_CreateDate"] = createDate,
        ["vnp_ExpireDate"] = DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss"),
    };

    var queryString = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
    var rawHash = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
    using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));
    var hash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash))).ToLower();

    var context = new OrderSerRedisContext();
    var txn = new Transaction
    {
        OrderId   = order.OrderId,
        Type      = "PAYMENT",
        Direction = "DEBIT",
        Amount    = order.TotalAmount,
        Status    = "PENDING",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };
    context.Transactions.Add(txn);
    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        success     = true,
        orderNumber = order.OrderId,
        amount      = order.TotalAmount,
        paymentUrl  = $"{baseUrl}?{queryString}&vnp_SecureHash={hash}"
    });
});

app.MapGet("/payment/vnpay/ipn", async (HttpRequest request, OrderStore orderStore, IConfiguration config) =>
{
    var query = request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
    var hashSecret = config["Vnpay:HashSecret"]!;

    query.TryGetValue("vnp_SecureHash", out var receivedHash);
    var signParams = new SortedDictionary<string, string>(
        query.Where(k => k.Key.StartsWith("vnp_") && k.Key != "vnp_SecureHash")
             .ToDictionary(k => k.Key, v => v.Value));
    var rawHash = string.Join("&", signParams.Select(kv => $"{kv.Key}={kv.Value}"));
    using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));
    var calcHash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash))).ToLower();

    if (!string.Equals(calcHash, receivedHash, StringComparison.OrdinalIgnoreCase))
        return Results.Ok(new { RspCode = "97", Message = "Invalid signature" });

    query.TryGetValue("vnp_TxnRef", out var orderId);
    query.TryGetValue("vnp_ResponseCode", out var responseCode);
    query.TryGetValue("vnp_TransactionStatus", out var txnStatus);

    var isSuccess = responseCode == "00" && txnStatus == "00";

    var context = new OrderSerRedisContext();
    var order = await context.Orders
        .Include(o => o.Transactions)
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order is null)
        return Results.Ok(new { RspCode = "01", Message = "Order not found" });

    var txn = order.Transactions.FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");
    if (txn != null)
    {
        txn.Status    = isSuccess ? "SUCCESS" : "FAILED";
        txn.UpdatedAt = DateTime.UtcNow;
    }
    order.Status    = isSuccess ? "CONFIRMED" : "CANCELLED";
    order.UpdatedAt = DateTime.UtcNow;
    await context.SaveChangesAsync();

    return Results.Ok(new { RspCode = "00", Message = "Confirmed" });
});

app.MapGet("/payment/vnpay/return", (HttpRequest request) =>
{
    var qs = request.QueryString.Value;
    return Results.Ok(new { message = "Payment return received", query = qs });
});

app.MapPost("/payment/vnpay/dev/simulate", async (VnpaySimulateRequest req, OrderStore orderStore) =>
{
    var context = new OrderSerRedisContext();
    var order = await context.Orders
        .Include(o => o.Transactions)
        .FirstOrDefaultAsync(o => o.Id == req.OrderNumber);

    if (order is null)
        return Results.NotFound(new { success = false, message = $"Order '{req.OrderNumber}' not found" });

    var txn = order.Transactions.FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");
    if (txn != null)
    {
        txn.Status    = req.Success ? "SUCCESS" : "FAILED";
        txn.UpdatedAt = DateTime.UtcNow;
    }
    order.Status    = req.Success ? "CONFIRMED" : "CANCELLED";
    order.UpdatedAt = DateTime.UtcNow;
    await context.SaveChangesAsync();

    return Results.Ok(new { success = true, orderStatus = order.Status });
});

app.Run();

record VnpayPayRequest(string OrderNumber);
record VnpaySimulateRequest(string OrderNumber, bool Success = true);