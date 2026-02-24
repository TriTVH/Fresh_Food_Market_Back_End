using AuthService.Model.DBContext;
using AuthService.Repository;
using AuthService.Repository.Implementor;
using AuthService.Service;
using AuthService.Service.Implementor;
using JwtConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AccountMgmtFfmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddJwtAuthentication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {

        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");

    });
}


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
