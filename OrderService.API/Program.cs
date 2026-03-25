using JwtConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderService.Model.DBContext;
using OrderService.Repository;
using OrderService.Repository.Implementor;
using OrderService.Service;
using OrderService.Service.HttpClients;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<OrderMgmtFfmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        })
);

// HTTP client for ProductCatalogService
builder.Services.AddHttpClient<IProductHttpClient, ProductHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductCatalogService"]!);
});

builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService.Service.Implementor.OrderService>();

builder.Services.AddJwtAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
