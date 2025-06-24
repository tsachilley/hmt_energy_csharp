using Consul;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class ConsulRegisterService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConsulService _consulService;
        private readonly ILogger<ConsulRegisterService> _logger;
        private EntityConsulServiceInfo _entityCSI;

        public ConsulRegisterService(IConfiguration configuration, IConsulClient consulClient, IConsulService consulService, ILogger<ConsulRegisterService> logger)
        {
            _logger = logger;
            _consulClient = consulClient;
            _consulService = consulService;
            _entityCSI = new EntityConsulServiceInfo();
            var csi = configuration.GetSection("ConsulServiceInfo");
            _entityCSI.Id = csi["Id"];
            _entityCSI.Name = csi["Name"];
            _entityCSI.IP = csi["IP"];
            _entityCSI.Port = Convert.ToInt32(csi["Port"]);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"start to register service {_entityCSI.Id} to consul client ...");
                await _consulClient.Agent.ServiceDeregister(_entityCSI.Id, cancellationToken);
                await _consulClient.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    ID = _entityCSI.Id,
                    Name = _entityCSI.Name,
                    Address = _entityCSI.IP,
                    Port = _entityCSI.Port,
                    Check = new AgentServiceCheck
                    {
                        GRPC = $"{_entityCSI.IP}:{_entityCSI.Port}",
                        //GRPCUseTLS = false,
                        //TLSSkipVerify = true,
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        Timeout = TimeSpan.FromSeconds(3600),
                        Interval = TimeSpan.FromSeconds(5)
                    }
                }, cancellationToken);
                Console.WriteLine("register service info to consul client Successful ...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Task.Factory.StartNew(async () =>
                {
                    await _consulService.ReRegister(_entityCSI, cancellationToken);
                });
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _consulClient.Agent.ServiceDeregister(_entityCSI.Id, cancellationToken);
                Console.WriteLine($"Deregister service {_entityCSI.Id} from consul client Successful ...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    /// <summary>
    /// Consul配置信息实体类
    /// </summary>
    public class EntityConsulServiceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public interface IConsulService
    {
        Task<List<AgentService>> GetServicesAsync(string serviceName);

        Task<GrpcChannel> GetGrpcChannelAsync(string serviceName);

        Task ReRegister(EntityConsulServiceInfo _entityCSI, CancellationToken cancellationToken);
    }

    public class ConsulService : IConsulService
    {
        private readonly ILogger<ConsulService> _logger;
        private readonly IConsulClient _client;

        public ConsulService(ILogger<ConsulService> logger, IConsulClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<List<AgentService>> GetServicesAsync(string serviceName)
        {
            try
            {
                var result = await _client.Health.Service(serviceName, "", true);
                return result.Response.Select(s => s.Service).ToList();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return new List<AgentService>();
        }

        public async Task<GrpcChannel> GetGrpcChannelAsync(string serviceName)
        {
            try
            {
                var services = await GetServicesAsync(serviceName);
                var service = services[new Random().Next(0, services.Count)];
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var options = new GrpcChannelOptions()
                {
                    HttpHandler = handler,
#if DEBUG
                    UnsafeUseInsecureChannelCallCredentials = true
#endif
                };
                var channel = GrpcChannel.ForAddress($"http://{service.Address}:{service.Port}", options);
                return channel;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return null;
        }

        public async Task ReRegister(EntityConsulServiceInfo _entityCSI, CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(60 * 1000, cancellationToken);
                try
                {
                    var lst = await GetServicesAsync(_entityCSI.Name);
                    if (lst == null || lst.Count < 1)
                    {
                        Console.WriteLine($"start to reregister service {_entityCSI.Id} to consul client ...");
                        await _client.Agent.ServiceRegister(new AgentServiceRegistration
                        {
                            ID = _entityCSI.Id,
                            Name = _entityCSI.Name,
                            Address = _entityCSI.IP,
                            Port = _entityCSI.Port,
                            Check = new AgentServiceCheck
                            {
                                GRPC = $"{_entityCSI.IP}:{_entityCSI.Port}",
                                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                                Timeout = TimeSpan.FromSeconds(3600),
                                Interval = TimeSpan.FromSeconds(5)
                            }
                        }, cancellationToken);
                        Console.WriteLine("reregister service info to consul client Successful ...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}