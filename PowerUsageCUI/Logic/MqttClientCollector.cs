using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PowerUsageCUI.Logic
{
    /// <summary>
    /// Class for Mqtt data collection
    /// </summary>
    public class MqttClientCollector : IMqttClientCollector, IDisposable 
    {
        private ILogger<MqttClientCollector> logger;
        private IMqttClient mqttClient;
        private CancellationTokenSource cancellationTokenSource;

        public delegate Task MqttMessageReceived(object sender, MqttTransactionEventArgs e);
        /// <summary>
        /// Event when a MQTT message is received
        /// </summary>
        public event MqttMessageReceived OnMqttMessageRecevied;


        public MqttClientCollector(ILogger<MqttClientCollector> logger)
        {
            Guard.Against.Null(logger);
            this.logger = logger;    
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Guard.Against.Null(cancellationToken);
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await setupMqttClientConnection();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Should already be handled
        }

        private async Task setupMqttClientConnection()
        {
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("192.168.1.22").Build();

            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;

            await mqttClient.ConnectAsync(mqttClientOptions, cancellationTokenSource.Token);

            // Subscribe to topics
            // __TODO__ convert to a list which can be changed from outside.
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("zigbee2mqtt/Zonnepanelen energie schuur");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("energy/growatt/PYL0CH1037");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("energy/growatt/QUH4BJS04P");
                    })
                .Build();

            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationTokenSource.Token);
            Console.WriteLine("MQTT client subscribed to topics.");

        }

        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            // Update it to our own event
            if (OnMqttMessageRecevied != null)
            {
                OnMqttMessageRecevied(this, new MqttTransactionEventArgs(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload)));
            }
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (mqttClient != null)
            {
                if (mqttClient.IsConnected) mqttClient.DisconnectAsync();
                mqttClient.Dispose();
            }
            
        }
    }

    public class MqttTransactionEventArgs(string topic, string payload) : EventArgs
    {
        public string Topic { get; private set; } = topic;
        public string Payload { get; private set; } = payload;
    }
}
