using BlogService.Model.DBContext;
using BlogService.Repository;
using BlogService.Repository.Implementor;
using BlogService.Service;
using BlogService.Service.Implementor;
using JwtConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<ContentMgmtFfmContext>(options =>
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

builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddHttpClient<INewsService, NewsService>(client =>
{
    var baseUrl = builder.Configuration["ExternalServices:ProductCatalogBaseUrl"] ?? "http://localhost:7000/";
    var runningInContainer = string.Equals(
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

    if (runningInContainer &&
        (baseUrl.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
         baseUrl.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase)))
    {
        baseUrl = "http://productcatalogservice.api:7000/";
    }

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddJwtAuthentication();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        new List<string>()
    }
});
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog API", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API V1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();