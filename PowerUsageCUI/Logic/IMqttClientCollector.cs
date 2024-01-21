using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PowerUsageCUI.Logic.MqttClientCollector;

namespace PowerUsageCUI.Logic
{
    public interface IMqttClientCollector
    {
        public event MqttMessageReceived OnMqttMessageRecevied;
        public Task StartAsync(CancellationToken cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken);

    }
}
