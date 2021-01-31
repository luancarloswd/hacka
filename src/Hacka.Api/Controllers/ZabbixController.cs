using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Hacka.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hacka.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZabbixController : ControllerBase
    {
        private readonly ILogger<ZabbixController> _logger;
        private readonly IConfiguration _configuration;

        private readonly HttpClient _httpClient;

        private const string UserZabixConfig = "UserZabbix";
        private const string PassZabbix = "PassZabbix";
        private readonly ICollection<EventZabbixParams> _data;

        public ZabbixController(ILogger<ZabbixController> logger, IConfiguration configuration, List<EventZabbixParams> data)
        {
            _logger = logger;
            _configuration = configuration;
            _data = data;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // var zabbix = new Zabbix.Zabbix(_configuration.GetValue<string>(UserZabixConfig),
            //     _configuration.GetValue<string>(PassZabbix),
            //     "https://opti.xpinc.io/zabbix-hml/api_jsonrpc.php");
            // await zabbix.LoginAsync();
            //
            // var response = await zabbix.GetResponseJsonAsync("event.get", new
            // {
            //     output = "extend",
            //     select_acknowledges = "extend",
            //     limit = 10,
            //     selectTags = "extend",
            //     selectSuppressionData = "extend",
            //     sortfield = new [] {"clock", "eventid"},
            //     sortorder = "DESC"
            // });
            // return Ok(response);

            return Ok(_data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventZabbixParams data)
        {
            _data.Add(data);
            if (data.IsAcknowledged)
            {
                await SendProblemTeams(data);
            }

            return Ok(data);
        }


        [HttpPost("sendProblem")]
        public IActionResult PostProblem()
        {
            SendProblemTeams(new EventZabbixParams());

            return Ok();
        }

        [HttpPost("inAnalysis")]
        public IActionResult PostInAnalysisTeams()
        {
            InAnalysisTeams("Order", "Hacka", "High");

            return Ok();
        }

        [HttpPost("resolved")]
        public IActionResult PostResolvedTeams()
        {
            ResolvedTeams("Order", "Hacka", "High");

            return Ok();
        }

        private async Task SendProblemTeams(EventZabbixParams eventZabbix)
        {
            var json = $@"{{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': 'ff0000',
                'summary': 'PROBLEM',
                'sections': [{{
                            'activityTitle': 'PROBLEM',
                    'facts': [{{
                                'name': 'Problem started:',
                        'value': '{eventZabbix.EventTime} on {eventZabbix.EventDate}'
                    }}, {{
                                'name': 'Problem name:',
                        'value': '{eventZabbix.AlertSubject}'
                         }}, {{
                                'name': 'Host:',
                        'value': '{eventZabbix.HostName}'
                         }}, {{
                                'name': 'Severity:',
                        'value': '{eventZabbix.EventSeverity}'
                         }}],
                    'markdown': true
                }}],
                'potentialAction': [
                {{
                            '@type': 'ActionCard',
                    'name': 'Change status',
                    'inputs': [{{
                                '@type': 'MultichoiceInput',
                        'id': 'list',
                        'title': 'Select a status',
                        'isMultiSelect': 'false',
                        'choices': [{{
                                    'display': 'In Analysis',
                            'value': '1'
                        }}, {{
                                    'display': 'Resolved',
                            'value': '2'
                             }}]
                    }}],
                    'actions': [{{
                                '@type': 'HttpPOST',
                        'name': 'Save',
                        'target': 'https://yammer.com/comment?postId=123',
                        'body': 'comment={{{{list.value}}}}'
                    }}]
                }}, 
                {{
                            '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    {{ 'os': 'default', 'uri': 'https://docs.microsoft.com/outlook/actionable-messages' }}
                  ]
                }}]}}".Replace("'", "\"");

            //var aeess = await "https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072"
            //                    .PostJsonAsync(json)
            //                        .ReceiveJson<int>()
            //                        .ConfigureAwait(false);

            await _httpClient.PostAsync("https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072", new StringContent(json, Encoding.UTF8, "application/json"));

        }


        private async void InAnalysisTeams(string problemName, string host, string severity)
        {
            var json = @"{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': 'ffff00',
                'summary': 'IN ANALYSIS',
                'sections': [{
                    'activityTitle': 'IN ANALYSIS',
                    'facts': [{
                        'name': 'Problem started:',
                        'value': '10:51:58 on 2021.01.30'
                    }, {
                        'name': 'Problem name:',
                        'value': 'URL http://sak-front.xpi.com.br:21607/api/v1/monitor/checkUpdatedCounterparty is not available'
                    }, {
                        'name': 'Host:',
                        'value': 'APISAK-FRONT21607_CheckUpdatedCounterparty'
                    }, {
                        'name': 'Severity:',
                        'value': 'P4 - Warning'
                    }],
                    'markdown': true
                }],
                'potentialAction': [
                {
                  '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    { 'os': 'default', 'uri': 'https://docs.microsoft.com/outlook/actionable-messages' }
                  ]
                }]
            }".Replace("'", "\"");

            await _httpClient.PostAsync("https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072", new StringContent(json, Encoding.UTF8, "application/json"));

        }

        private async void ResolvedTeams(string problemName, string host, string severity)
        {
            var json = @"{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': '008000',
                'summary': 'RESOLVED',
                'sections': [{
                    'activityTitle': 'RESOLVED',
                    'facts': [{
                        'name': 'Problem started:',
                        'value': '10:51:58 on 2021.01.30'
                    }, {
                        'name': 'Problem name:',
                        'value': 'URL http://sak-front.xpi.com.br:21607/api/v1/monitor/checkUpdatedCounterparty is not available'
                    }, {
                        'name': 'Host:',
                        'value': 'APISAK-FRONT21607_CheckUpdatedCounterparty'
                    }, {
                        'name': 'Severity:',
                        'value': 'P4 - Warning'
                    }],
                    'markdown': true
                }],
                'potentialAction': [
                {
                  '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    { 'os': 'default', 'uri': 'https://docs.microsoft.com/outlook/actionable-messages' }
                  ]
                }]
            }".Replace("'", "\"");

            await _httpClient.PostAsync("https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072", new StringContent(json, Encoding.UTF8, "application/json"));

        }
    }
}