using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hacka.Domain;
using Hacka.Domain.Abstractions;

namespace Hacka.Api.BackgroundServices
{
    public class VerifyAlertsBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<VerifyAlertsBackgroundService> _logger;
        private Timer _timer;

        private readonly IEventZabbixRepository _eventZabbixRepository;
        private readonly IZabbixRepository _zabbixRepository;
        private readonly IMsTeamsRepository _msTeamsRepository;
        public VerifyAlertsBackgroundService(
            ILogger<VerifyAlertsBackgroundService> logger,
            IEventZabbixRepository eventZabbixRepository, IZabbixRepository zabbixRepository, IMsTeamsRepository msTeamsRepository)
        {
            _logger = logger;
            _eventZabbixRepository = eventZabbixRepository;
            _zabbixRepository = zabbixRepository;
            _msTeamsRepository = msTeamsRepository;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            //_timer = new Timer(async state => await VerifyAlerts(state), null, TimeSpan.Zero,
            //    TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async Task VerifyAlerts(object state)
        {
            try
            {
                var events = await _eventZabbixRepository.GetAllAsync(a => a.InAnalisys != true);
                foreach (var eventZabbix in events)
                {
                    var statusActual = await _zabbixRepository.GetActualStatusEvent(eventZabbix.EventId);
                    if (statusActual == EStatusEvent.Problem)
                    {
                        await _msTeamsRepository.SendProbleamToSquad(eventZabbix);
                    }
                }
            }
            catch
            {
                //Do nothing
            }
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
