using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Services
{
    public class MqttService : IHostedService
    {
        public IMqttClient MC { get; set; }
        public bool ConnectStatus { get; set; }

        private readonly IConfiguration _configuration;

        public MqttService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Connect();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            ConnectStatus = false;
            if (MC.IsConnected)
            {
                await MC.DisconnectAsync();
                MC.Dispose();
            }
        }

        public async Task<MqttClientConnectResult> Connect()
        {
            var b = new MqttClientOptionsBuilder()
                .WithTcpServer(_configuration["Mqtt:Ip"], Convert.ToInt32(_configuration["Mqtt:Port"]))
                .WithCredentials(_configuration["Mqtt:Username"], _configuration["Mqtt:Password"])
                .WithClientId(_configuration["Mqtt:ClientId"])
                .WithCleanSession()
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false
                });
            var options = b.Build();

            MC = new MqttFactory().CreateMqttClient();
            MC.ConnectedAsync += MC_ConnectedAsync;
            MC.DisconnectedAsync += MC_DisconnectedAsync;

            return await MC.ConnectAsync(options);
        }

        public async Task Publish(string data)
        {
            if (MC.IsConnected)
            {
                var message = new MqttApplicationMessage
                {
                    Topic = _configuration["Mqtt:Topic"],
                    PayloadSegment = Encoding.UTF8.GetBytes(data),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
                    Retain = false
                };
                await MC.PublishAsync(message);
            }
        }

        private Task MC_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            if (ConnectStatus)
            {
                MC.Dispose();
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        var result = await Connect();
                        if (result.ResultCode == MqttClientConnectResultCode.Success)
                            break;
                        await Task.Delay(1000 * 10);
                    }
                });
            }
            return Task.CompletedTask;
        }

        private Task MC_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            ConnectStatus = true;
            return Task.CompletedTask;
        }
    }
}