using Hacka.Domain;
using Hacka.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Hacka.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZabbixController : ControllerBase
    {
        private readonly IEventZabbixRepository _eventZabbixRepository;
        private readonly ISquadRepository _squadRepository;
        private readonly IMsTeamsRepository _msTeamsRepository;

        public ZabbixController(IEventZabbixRepository eventZabbixRepository, ISquadRepository squadRepository, IMsTeamsRepository msTeamsRepository)
        {
            _eventZabbixRepository = eventZabbixRepository;
            _squadRepository = squadRepository;
            _msTeamsRepository = msTeamsRepository;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventZabbixParams data)
        {
            var eventExists = await _eventZabbixRepository.GetByIdAsync(data.EventId);
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

            if (data.IsAcknowledged)
            {
                await _msTeamsRepository.SendProbleamToSquad(data);
            }

            return Ok(data);
        }

        

        [HttpPost("inAnalysis/{eventId}")]
        public async Task<IActionResult> PostInAnalysisTeams(string eventId)
        {
            var zabbixEvent = await _eventZabbixRepository.GetByIdAsync(eventId);

            if (zabbixEvent == default || zabbixEvent.InAnalisys == true) return Ok();

            zabbixEvent.InAnalisys = true;
            await _eventZabbixRepository.UpdateAsync(zabbixEvent);
            await _msTeamsRepository.SendInAnalisys(zabbixEvent);

            return Ok();
        }
    }
}