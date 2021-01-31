using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Hacka.Domain;
using Hacka.Domain.Abstractions;
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
        private const string TEAMS_HOOK_URL = "https://hackaapi20210130155201.azurewebsites.net/Zabbix/inAnalysis";

        private readonly HttpClient _httpClient;

        private const string UserZabixConfig = "UserZabbix";
        private const string PassZabbix = "PassZabbix";
        private readonly ICollection<EventZabbixParams> _data;
        private readonly IEventZabbixRepository _eventZabbixRepository;
        private readonly ISquadRepository _squadRepository;

        public ZabbixController(ILogger<ZabbixController> logger, IConfiguration configuration, List<EventZabbixParams> data,
            IEventZabbixRepository  eventZabbixRepository, ISquadRepository squadRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _data = data;
            _httpClient = new HttpClient();
            _eventZabbixRepository = eventZabbixRepository;
            _squadRepository = squadRepository;
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

            return Ok(await _eventZabbixRepository.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventZabbixParams data)
        {
            var eventExists = await _eventZabbixRepository.GetByIdAsync(data.EventId);
            var squadNameOnTag = GetSquadName(data);
            var squad = await _squadRepository.GetByNameAsync(squadNameOnTag);
            if (eventExists != default)
            {
                eventExists.AlertMessage = data.AlertMessage;
                eventExists.AlertSubject = data.AlertSubject;
                eventExists.EventDate = data.EventDate;
                eventExists.EventId = data.EventId;
                eventExists.EventNseverity = data.EventNseverity;
                eventExists.EventOpdata = data.EventOpdata;
                eventExists.EventRecoveryDate = data.EventRecoveryDate;
                eventExists.EventRecoveryTime = data.EventRecoveryTime;
                eventExists.EventSeverity = data.EventSeverity;
                eventExists.EventSource = data.EventSource;
                eventExists.EventStatus = data.EventStatus;
                eventExists.EventTags = data.EventTags;
                eventExists.EventTime = data.EventTime;
                eventExists.EventUpdateAction = data.EventUpdateAction;
                eventExists.EventUpdateDate = data.EventUpdateDate;
                eventExists.Endpoint = data.Endpoint;
                eventExists.EventUpdateMessage = data.EventUpdateMessage;
                eventExists.EventUpdateStatus = data.EventUpdateStatus;
                eventExists.EventUpdateTime = data.EventUpdateTime;
                eventExists.EventUpdateUser = data.EventUpdateUser;
                eventExists.EventValue = data.EventValue;
                eventExists.HostIp = data.HostIp;
                eventExists.HostName = data.HostName;
                eventExists.ItemId = data.ItemId;
                eventExists.TriggerDescription = data.TriggerDescription;
                eventExists.TriggerId = data.TriggerId;
                eventExists.ZabbixUrl = data.ZabbixUrl;
                await _eventZabbixRepository.UpdateAsync(eventExists);
            }
            else
            {
                await _eventZabbixRepository.AddAsync(data);
            }

            if (data.IsAcknowledged && squad != default)
            {
                await SendProblemTeams(data, squad);
            }

            return Ok(data);
        }

        private string GetSquadName(EventZabbixParams data)
        {
            var splited = data.EventTags.Split(',');
            var squadTag =  splited.FirstOrDefault(s => s.Contains("SQUAD"));
            return squadTag?.Replace("SQUAD:", string.Empty);
        }


        [HttpPost("sendProblem")]
        public IActionResult PostProblem()
        {
            SendProblemTeams(new EventZabbixParams(), new Squad());

            return Ok();
        }

        [HttpPost("inAnalysis/{eventId}")]
        public async Task<IActionResult> PostInAnalysisTeams(string eventId)
        {
            var zabbixEvent = await _eventZabbixRepository.GetByIdAsync(eventId);
            var squadNameOnTag = GetSquadName(zabbixEvent);
            var squad = await _squadRepository.GetByNameAsync(squadNameOnTag);

            if (zabbixEvent != default && squad != default)
            {
                await InAnalysisTeams(zabbixEvent, squad);
            }

            return Ok();
        }

        [HttpPost("resolved")]
        public IActionResult PostResolvedTeams()
        {
            ResolvedTeams("Order", "Hacka", "High");

            return Ok();
        }

        private async Task SendProblemTeams(EventZabbixParams eventZabbix, Squad squad)
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
                        'target': '{TEAMS_HOOK_URL}/{eventZabbix.EventId}',
                        'headers': [{{
                            'name': 'content-type',
                            'value': 'application/json'
                        }}]
                    }}]
                }}, 
                {{
                            '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    {{ 'os': 'default', 'uri': '{GetUrl(eventZabbix)}' }}
                  ]
                }}]}}".Replace("'", "\"");

            await _httpClient.PostAsync(squad.ChannelTeams, new StringContent(json, Encoding.UTF8, "application/json"));

        }

        private string GetUrl(EventZabbixParams eventZabbix) =>
            eventZabbix.EventSource == "0"
                ? $"{eventZabbix.ZabbixUrl}/tr_events.php?triggerid={eventZabbix.TriggerId}&eventid={eventZabbix.EventId}"
                : eventZabbix.ZabbixUrl;


        private async Task InAnalysisTeams(EventZabbixParams eventZabbix, Squad squad)
        {
            var json = $@"{{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': 'ffff00',
                'summary': 'IN ANALYSIS',
                'sections': [{{
                    'activityTitle': 'IN ANALYSIS',
                    'facts': [{{
                        'name': 'Problem started:',
                        'value': '10:51:58 on 2021.01.30'
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
                  '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    {{ 'os': 'default', 'uri': '{GetUrl(eventZabbix)}' }}
                  ]
                }}]
            }}".Replace("'", "\"");

            await _httpClient.PostAsync(squad.ChannelTeams, new StringContent(json, Encoding.UTF8, "application/json"));

        }

        private async void ResolvedTeams(string problemName, string host, string severity)
        {
            var json = $@"{{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': '008000',
                'summary': 'RESOLVED',
                'sections': [{{
                    'activityTitle': 'RESOLVED',
                    'facts': [{{
                        'name': 'Problem started:',
                        'value': '10:51:58 on 2021.01.30'
                    }}, {{
                        'name': 'Problem name:',
                        'value': '{problemName}'
                    }}, {{
                        'name': 'Host:',
                        'value': '{host}'
                    }}, {{
                        'name': 'Severity:',
                        'value': '{severity}'
                    }}],
                    'markdown': true
                }}],
                'potentialAction': [
                {{
                  '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    {{ 'os': 'default', 'uri': 'https://docs.microsoft.com/outlook/actionable-messages' }}
                  ]
                }}]
            }}".Replace("'", "\"");

            await _httpClient.PostAsync("https://xpcorretora.webhook.office.com/webhookb2/9e74fc8a-77ec-4c42-9c8c-2590bcf0492f@cf56e405-d2b0-4266-b210-aa04636b6161/IncomingWebhook/67721fccac1743c49ca54452f68d33b0/d9be2d45-f7de-458f-9fa7-f346a435d072", new StringContent(json, Encoding.UTF8, "application/json"));

        }
    }
}