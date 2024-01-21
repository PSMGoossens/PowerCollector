using Ardalis.GuardClauses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PowerUsageCUI.DataModel.Configuration;
using PowerUsageCUI.DataModel.PowerModel;
using PowerUsageCUI.DsmrReader;
using PowerUsageCUI.DsmrReader.Models;
using PowerUsageCUI.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerUsageCUI.Services
{
    public class CollectorHostService : IHostedService
    {

        private readonly ILogger<CollectorHostService> _logger;
        private IDsmrParser dsmrParser;
        private IP1HttpCollector p1HttpCollector;
        private StatisticsReporterService statisticsReporter;
        private CancellationTokenSource cancellationTokenSource = null;
        private IMqttClientCollector mqttClientCollector;

        public CollectorHostService(ILogger<CollectorHostService> logger, 
            IDsmrParser dsmrParser,
            IP1HttpCollector p1HttpCollector,
            StatisticsReporterService statisticsReporter,
            IMqttClientCollector mqttClientCollector)
        {
            Guard.Against.Null(logger);
            Guard.Against.Null(dsmrParser);
            Guard.Against.Null(p1HttpCollector);
            Guard.Against.Null(statisticsReporter);
            Guard.Against.Null(mqttClientCollector);

            this._logger = logger;
            this.dsmrParser = dsmrParser;
            this.p1HttpCollector = p1HttpCollector;
            this.statisticsReporter = statisticsReporter;
            this.mqttClientCollector = mqttClientCollector;
            _logger.LogDebug("Setup CollectorHostService");

            setup();           
        }

        /// <summary>
        /// Setup some states in this objct
        /// </summary>
        private void setup()
        {
            this.p1HttpCollector.OnTelegramReceived += P1HttpCollector_OnTelegramReceived;
            this.mqttClientCollector.OnMqttMessageRecevied += MqttClientCollector_OnMqttMessageRecevied;
        }

        /// <summary>
        /// Start the services async.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await p1HttpCollector.StartAsync(cancellationToken);
            await mqttClientCollector.StartAsync(cancellationToken);

            await Task.Run(() =>
            {
                Console.WriteLine("Press 'c' to cancel the task.");
                Console.WriteLine("Or let the task time out by doing nothing.");
                if (Console.ReadKey(true).KeyChar == 'c')
                    cancellationTokenSource.Cancel();
            });
        }

        /// <summary>
        /// Stop the services related to this object.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            p1HttpCollector.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }



        /// <summary>
        /// Event handler for p1 telegram from p1 meter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task P1HttpCollector_OnTelegramReceived(object sender, HttpReceivedEventArgs e)
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                _logger.LogInformation("Received P1 telegram. Start decoding");
                var telegrams = await dsmrParser.Parse(e.Body);
                if (telegrams?.Count() > 0)
                {
                    var firstTelegram = telegrams.First();
                    var threePhaseData = (ThreePhaseMeter)firstTelegram;
                    _logger.LogInformation(string.Format("{0:H:mm:ss} active power {1:0.00} Watt", threePhaseData.DateTime, threePhaseData.TotalActivePower));
                    await writeToDatabase(threePhaseData);

                    statisticsReporter.ShowStatistics();
                } 
                else
                {
                    _logger.LogWarning("Cannot parse reveived p1 telegram");
                }
            }
        }

        /// <summary>
        /// Handle the received Mqtt message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task MqttClientCollector_OnMqttMessageRecevied(object sender, MqttTransactionEventArgs e)
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                _logger.LogInformation($"Recevied message from Mqtt with topic {e.Topic}.");

                // Deserialize the information based on topic or type
                var result = JsonConvert.DeserializeObject<DataModel.Zigbee2Mqtt.ThreePhaseMeter>(e.Payload);

                if (result != null)
                {
                    // Log some info to console.
                    _logger.LogInformation($"Total active power for topic {e.Topic}: {result.TotalPower} Watt, " +
                        $"Phase X: {result.PowerX} W, Phase Y: {result.PowerY} W, Phase Z: {result.PowerZ} W.");
                }
            }
        }

        /// <summary>
        /// Write the collection to the database
        /// </summary>
        /// <param name="threePhaseMeter"></param>
        /// <returns></returns>
        private async Task writeToDatabase(ThreePhaseMeter threePhaseMeter)
        {
            // __TODO__ [PSMG} concurrency handling when committing. Especally with debugging
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                using var db = new PowerModelContext();

                // Check if item already exist.. Then update. Otherwise add
                var existingRecord = db.ThreePhaseMeterSet.Where(pred =>
                pred.DateTime == threePhaseMeter.DateTime &&
                pred.DeviceName == threePhaseMeter.DeviceName).FirstOrDefault();

                if (existingRecord != null)
                {
                    // Update record
                    //db.ThreePhaseMeterSet.Update(threePhaseMeter);
                }
                else
                {
                    // Add new record
                    await db.ThreePhaseMeterSet.AddAsync(threePhaseMeter);
                }

                await db.SaveChangesAsync();
            }
        }

    }
}
