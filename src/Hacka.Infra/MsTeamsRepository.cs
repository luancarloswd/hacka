using Flurl.Http;
using Hacka.Domain;
using Hacka.Domain.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hacka.Infra
{
    public class MsTeamsRepository : IMsTeamsRepository
    {
        private readonly ISquadRepository _squadRepository;
        private const string TeamsHookUrl = "https://hackaapi20210130155201.azurewebsites.net/Zabbix/inAnalysis";

        public MsTeamsRepository(ISquadRepository squadRepository)
        {
            _squadRepository = squadRepository;
        }

        public async Task SendProbleamToSquad(EventZabbixParams eventZabbix)
        {
            var squadNameOnTag = GetSquadName(eventZabbix);
            var squad = await _squadRepository.GetByNameAsync(squadNameOnTag);
            if (squad == default) return;
            
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
                        'target': '{TeamsHookUrl}/{eventZabbix.EventId}',
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

            await squad.ChannelTeams.PostAsync(new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private string GetUrl(EventZabbixParams eventZabbix) =>
            eventZabbix.EventSource == "0"
                ? $"{eventZabbix.ZabbixUrl}/tr_events.php?triggerid={eventZabbix.TriggerId}&eventid={eventZabbix.EventId}"
                : eventZabbix.ZabbixUrl;

        private string GetSquadName(EventZabbixParams data)
        {
            var splited = data.EventTags.Split(',');
            var squadTag = splited.FirstOrDefault(s => s.Contains("SQUAD"));
            return squadTag?.Replace("SQUAD:", string.Empty);
        }

        public async Task SendInAnalisys(EventZabbixParams eventZabbix)
        {
            var squadNameOnTag = GetSquadName(eventZabbix);
            var squad = await _squadRepository.GetByNameAsync(squadNameOnTag);
            if (squad == default) return;

            var json = $@"{{
                '@type': 'MessageCard',
                '@context': 'http://schema.org/extensions',
                'themeColor': 'ffff00',
                'summary': 'IN ANALYSIS',
                'sections': [{{
                    'activityTitle': 'IN ANALYSIS',
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
                  '@type': 'OpenUri',
                  'name': 'Vizualizar alerta',
                  'targets': [
                    {{ 'os': 'default', 'uri': '{GetUrl(eventZabbix)}' }}
                  ]
                }}]
            }}".Replace("'", "\"");

            await squad.ChannelTeams.PostAsync(new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}
