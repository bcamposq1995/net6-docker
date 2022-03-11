using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ALLOW_ALL",
                      builder =>
                      {
                          builder.WithOrigins("*").WithHeaders("*").WithMethods("*");
                      });
});
//Cors

builder.Services.AddControllers();

builder.Services.AddOcelot()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    })
    .AddConsul();
    

var app = builder.Build();
//Cors
app.UseCors("ALLOW_ALL");
//Cors
app.UseAuthorization();
app.MapControllers();
app.UseOcelot().Wait();
app.Run();

