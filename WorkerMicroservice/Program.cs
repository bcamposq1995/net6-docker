using System.Net;
using Microsoft.EntityFrameworkCore;
using People.HttpMicroservice.Services.Get;
using Prometheus;
using StackExchange.Redis;
using WorkerMicroservice;
using WorkerMicroservice.Repositories.Cache;
using WorkerMicroservice.Repositories.Database;
using WorkerMicroservice.Repositories.DataBase;
using WorkerMicroservice.ServiceRegistration;
using WorkerMicroservice.Services.Delete;
using WorkerMicroservice.Services.Patch;
using WorkerMicroservice.Services.Post;
using WorkerMicroservice.Services.Put;
using WorkerMicroservice.Services.Queue;
using Consul;

int port = 80;

#if DEBUG
    port = 5146;
#endif

var server = new MetricServer(hostname: Dns.GetHostName(), port: port);
server.Start();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        //Consul
        services.AddSingleton<IHostedService, ConsulHostedService>();
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
        #if DEBUG
            consulConfig.Address = new Uri("http://localhost:8500");
        #else
            consulConfig.Address = new Uri("http://consul:8500");
        #endif
        }));
        //Consul

        services.AddDbContext<PersonDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Server=localhost;Port=5432;Database=person;User Id=person;Password=123456789"), ServiceLifetime.Transient);
        services.AddScoped(x => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost").GetDatabase());
        services.AddTransient<IGetPersonService, GetPersonService>();
        services.AddTransient<IPostPersonService, PostPersonService>();
        services.AddTransient<IPutPersonService, PutPersonService>();
        services.AddTransient<IPatchPersonService, PatchPersonService>();
        services.AddTransient<IDeletePersonService, DeletePersonService>();
        services.AddTransient<IDatabaseRepository, DatabaseRepository>();
        services.AddTransient<ICacheRepository, CacheRepository>();

        services.AddTransient<IPostQueueService, PostQueueService>();
        services.AddTransient<IPutQueueService, PutQueueService>();
        services.AddTransient<IPatchQueueService, PatchQueueService>();
        services.AddTransient<IDeleteQueueService, DeleteQueueService>();
        services.AddTransient<IGetByIdQueueService, GetByIdQueueService>();
        services.AddTransient<IGetListQueueService, GetListQueueService>();

        services.AddHostedService<Worker>();
        
    })
    .Build();

await host.RunAsync();

