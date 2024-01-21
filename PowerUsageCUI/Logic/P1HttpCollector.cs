using Ardalis.GuardClauses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerUsageCUI.DataModel.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerUsageCUI.Logic
{

    /// <summary>
    /// Class that collects P1 telegram from Http server
    /// </summary>
    public class P1HttpCollector : IP1HttpCollector
    {
        private readonly ILogger<P1HttpCollector> _logger;
        private HttpClient httpClient;
        private P1Configuration p1Configuration;
        private Timer timer;
        private CancellationTokenSource cancellationTokenSource;


        public delegate Task TelegramReceived(object sender, HttpReceivedEventArgs e);
        /// <summary>
        /// Event when a P1 telegram is received
        /// </summary>
        public event TelegramReceived OnTelegramReceived;

        public P1HttpCollector(ILogger<P1HttpCollector> logger, 
            HttpClient httpClient,
            P1Configuration p1Configuration) 
        {
            Guard.Against.Null(logger);
            Guard.Against.Null(httpClient);
            Guard.Against.Null(p1Configuration);

            this._logger = logger;
            this.httpClient = httpClient;
            this.p1Configuration = p1Configuration;

            setupHttpClient();
        }

        /// <summary>
        /// Send a stop request
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Is already handled by the linked token.
        }

        /// <summary>
        /// Start the collection program
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _logger.LogInformation($"Start P1HttpCollector timer with interval of {p1Configuration.RequestInterval} seconds");
            timer = new Timer(executeHttpRequest, this.cancellationTokenSource, 0, p1Configuration.RequestInterval * 1000);
        }


        /// <summary>
        /// Setup the HttpClient with the required information
        /// </summary>
        private void setupHttpClient()
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }


        /// <summary>
        /// Execute the HTTP call and initiate the process
        /// </summary>
        /// <param name="state"></param>
        private async void executeHttpRequest(object? state)
        {

            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                string? httpResult = null;
                try
                {
                    httpResult = await httpClient.GetStringAsync("http://192.168.26.16/api/v1/telegram", cancellationTokenSource.Token);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogInformation(ex, "Http request exception");
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogInformation(ex, "Invalid Operation exception");
                    return;
                }
                if (string.IsNullOrWhiteSpace(httpResult))
                {
                    _logger.LogWarning($"No valid reponse from url:");
                }

                // Invoke an event
                if (OnTelegramReceived != null)
                {
                    await OnTelegramReceived(this, new HttpReceivedEventArgs(httpResult));
                }
            }
            else
            {
                // Stop the timer.
                await timer.DisposeAsync();
            }
        }
    }

    public class HttpReceivedEventArgs(string body) : EventArgs
    {
        public string Body { get; private set; } = body;
    }
}
