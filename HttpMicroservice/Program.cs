using People.HttpMicroservice.Filters;
using People.HttpMicroservice.Services.Delete;
using People.HttpMicroservice.Services.Get;
using People.HttpMicroservice.Services.Patch;
using People.HttpMicroservice.Services.Post;
using People.HttpMicroservice.Services.Put;
using Consul;
using HttpMicroservice.Repositories.Cache;
using HttpMicroservice.ServiceRegistration;
using Prometheus;
using StackExchange.Redis;
using People.HttpMicroservice.Repositories.Cache;
using HttpMicroservice.Repositories.Queue;

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

//Consul
builder.Services.AddSingleton<IHostedService, ConsulHostedService>();
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
#if DEBUG
    consulConfig.Address = new Uri("http://localhost:8500");
#else
    consulConfig.Address = new Uri("http://consul:8500");
#endif
}));
//Consul

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options => options.Filters.Add<HttpResponseExceptionFilter>());
builder.Services.AddScoped(x => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost").GetDatabase());
builder.Services.AddTransient<IGetPersonService, GetPersonService>();
builder.Services.AddTransient<IPostPersonService, PostPersonService>();
builder.Services.AddTransient<IPutPersonService, PutPersonService>();
builder.Services.AddTransient<IPatchPersonService, PatchPersonService>();
builder.Services.AddTransient<IDeletePersonService, DeletePersonService>();
builder.Services.AddTransient<ICacheRepository, CacheRepository>();
builder.Services.AddTransient<IQueueRepository, QueueRepository>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Prometheus
var counter = Metrics.CreateCounter("request_count", "Counts requests to the API endpoints", new CounterConfiguration
{
    LabelNames = new[] { "method", "endpoint" }
});

app.Use((context, next) =>
{
    counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
    return next();
});
//Prometheus

//Cors
app.UseCors("ALLOW_ALL");
//Cors

app.UseAuthorization();
app.MapControllers();
app.UseMetricServer();
app.UseHttpMetrics();


app.Run();

