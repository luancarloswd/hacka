using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hacka.Domain.Abstractions;

namespace Hacka.Api.BackgroundServices
{
    public class VerifyAlertsBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<VerifyAlertsBackgroundService> _logger;
        private Timer _timer;

        private readonly IEventZabbixRepository _zabbixRepository;

        public VerifyAlertsBackgroundService(
            ILogger<VerifyAlertsBackgroundService> logger,
            IEventZabbixRepository zabbixRepository)
        {
            _logger = logger;
            _zabbixRepository = zabbixRepository;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async state => await VerifyAlerts(state), null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async Task VerifyAlerts(object state)
        {
            var events = await _zabbixRepository.GetAllAsync(a => a.InAnalisys == false);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
