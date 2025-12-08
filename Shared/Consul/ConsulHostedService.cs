using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;


namespace Shared.Consul
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsulHostedService> _logger;
        private string _serviceId;

        public ConsulHostedService(
            IConsulClient consulClient,
            IConfiguration configuration,
            ILogger<ConsulHostedService> logger)
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var serviceConfig = _configuration.GetSection("ServiceSettings").Get<ConsulConfig>();

            if (serviceConfig == null)
            {
                throw new ArgumentNullException("ServiceSettings section is missing in configuration.");
            }

            // Force service ID & Address
            _serviceId = serviceConfig.ServiceId;

            var registration = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = serviceConfig.ServiceName,
                Address = serviceConfig.ServiceAddress,
                Port = serviceConfig.ServicePort,
                Checks = new[]
                {
                    new AgentServiceCheck
                    {
                        HTTP = $"http://{serviceConfig.ServiceAddress}:{serviceConfig.ServicePort}/health",
                        Interval = TimeSpan.FromSeconds(serviceConfig.HealthCheckIntervalSeconds),
                        Timeout = TimeSpan.FromSeconds(serviceConfig.HealthCheckTimeoutSeconds)
                    }
                }
            };

            // ────────────── Deregister any old service ──────────────
            _logger.LogInformation($"Deregistering old service if exists: {_serviceId}");
            await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);

            // ────────────── Register service ──────────────
            _logger.LogInformation($"Registering service with Consul: {registration.Name} at {registration.Address}:{registration.Port}");
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

            // ────────────── Load optional config from Consul KV ──────────────
            await LoadConfigFromConsulKV(serviceConfig.ServiceName, cancellationToken);
        }

        private async Task LoadConfigFromConsulKV(string key, CancellationToken cancellationToken)
        {
            var kv = _consulClient.KV;
            var result = await kv.Get(key, cancellationToken);
            if (result.Response != null && result.Response.Value != null)
            {
                var data = Encoding.UTF8.GetString(result.Response.Value);
                var configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                if (configData != null)
                {
                    foreach (var item in configData)
                    {
                        _configuration[item.Key] = item.Value;
                    }
                    _logger.LogInformation($"Loaded config from Consul KV for key: {key}");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deregistering service from Consul: {_serviceId}");
            await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
        }
    }


}

