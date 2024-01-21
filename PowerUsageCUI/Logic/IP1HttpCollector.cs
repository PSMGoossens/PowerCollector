using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PowerUsageCUI.Logic.P1HttpCollector;

namespace PowerUsageCUI.Logic
{
    public interface IP1HttpCollector
    {
        public event TelegramReceived OnTelegramReceived;
        public Task StartAsync(CancellationToken cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken);
    }
}
