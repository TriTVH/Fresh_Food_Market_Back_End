using CacheManager.Core;
using JwtConfiguration;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// Add services to the container.
builder.Configuration.AddJsonFile(
               "ocelot.json",
               optional: false,
               reloadOnChange: true
           );

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithJsonSerializer()
        .WithRedisConfiguration("redis", config =>
        {
            config.WithEndpoint("redis", 6379);
        })
        .WithRedisCacheHandle("redis");
    });


builder.Services.AddSwaggerForOcelot(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
