using System.Net;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;

namespace HttpMicroservice.ServiceRegistration
{
	public class ConsulHostedService : IHostedService
    {
        private CancellationTokenSource? _cts;
        private readonly IConsulClient? _consulClient;
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly string _registrationId = $"{Guid.NewGuid()}-person";

        public ConsulHostedService(IConsulClient consulClient, ILogger<ConsulHostedService> logger)
        {
            _logger = logger;
            _consulClient = consulClient;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            string host = Dns.GetHostName();
            int port = 80;

#if DEBUG
            port = 5145;
#endif

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationId,
                Name = "person",
                Address = host,
                Port = port,
                Tags = new string[] { "person" },
                Checks = new AgentServiceCheck[]
                {
                    new AgentServiceCheck
                    {
                        HTTP = $"http://{host}:{port}/",
                        Method = "head",
                        Interval = TimeSpan.FromSeconds(5),
                        Timeout = TimeSpan.FromSeconds(3),
                        Name = "Health check Person",
                        Notes = "Health check Person"
                    }
                }
            };

            _logger.LogInformation("Registering in Consul");
            await _consulClient!.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts!.Cancel();
            _logger.LogInformation("De-registering from Consul");
            try
            {
                await _consulClient!.Agent.ServiceDeregister(_registrationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"De-registration failed");
            }
        }
    }
}

