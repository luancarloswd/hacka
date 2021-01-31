using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hacka.Domain;
using Hacka.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using ZabbixAPICore;

namespace Hacka.Infra
{
    public class ZabbixRepository : IZabbixRepository
    {
        private const string UserZabixConfig = "UserZabbix";
        private const string PassZabbix = "PassZabbix";
        private const string UrlApiZabbix = "UrlApiZabbix";
        private readonly IConfiguration _configuration;

        public ZabbixRepository(IConfiguration configuration) => _configuration = configuration;

        public async Task<EStatusEvent> GetActualStatusEvent(string eventId)
        {
            var zabbix = new Zabbix(_configuration.GetValue<string>(UserZabixConfig),
                _configuration.GetValue<string>(PassZabbix),
                _configuration.GetValue<string>(UrlApiZabbix));
            await zabbix.LoginAsync();

            var response = await zabbix.GetResponseJsonAsync("event.get", new
            {
                output = "extend",
                selectHosts = "extend",
                selectRelatedObject = "extend",
                select_alerts = "extend",
                selectTags = "extend",
                selectSuppressionData = "extend",
                select_acknowledges = "extend",
                limit = 1,
                eventids = eventId,
                sortfield = new[] { "clock", "eventid" },
                sortorder = "DESC"
            });
            return default;
        }
    }
}
