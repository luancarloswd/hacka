using Xunit;

namespace Hacka.Domain.Test
{
    public class EventZabbixParamsTest
    {
        [Fact]
        public void Should_Instantiate_With_Success()
        {
            var instance = new EventZabbixParams()
            {
                AlertMessage = "test",
                AlertSubject = "test",
                EventDate = "test",
                EventId = "test",
                EventNseverity = "test",
                EventOpdata = "test",
                EventRecoveryDate = "test",
                EventRecoveryTime = "test",
                EventSeverity = "test",
                EventSource = "test",
                EventStatus = "test",
                EventTags = "test",
                EventTime = "test",
                EventUpdateAction = "test",
                EventUpdateDate = "test",
                Endpoint = "test",
                EventUpdateMessage = "test",
                EventUpdateStatus = "test",
                EventUpdateTime = "test",
                EventUpdateUser = "test",
                EventValue = "test",
                HostIp = "test",
                HostName = "test",
                ItemId = "test",
                TriggerDescription = "test",
                TriggerId = "test",
                ZabbixUrl = "test",
                InAnalisys = false,
            };

            Assert.IsType<EventZabbixParams>(instance);
            Assert.NotNull(instance);

            Assert.IsType<string>(instance.AlertMessage);
            Assert.IsType<string>(instance.AlertSubject);
            Assert.IsType<string>(instance.EventDate);
            Assert.IsType<string>(instance.EventId);
            Assert.IsType<string>(instance.EventNseverity);
            Assert.IsType<string>(instance.EventOpdata);
            Assert.IsType<string>(instance.EventRecoveryDate);
            Assert.IsType<string>(instance.EventRecoveryTime);
            Assert.IsType<string>(instance.EventSeverity);
            Assert.IsType<string>(instance.EventSource);
            Assert.IsType<string>(instance.EventStatus);
            Assert.IsType<string>(instance.EventTags);
            Assert.IsType<string>(instance.EventTime);
            Assert.IsType<string>(instance.EventUpdateAction);
            Assert.IsType<string>(instance.EventUpdateDate);
            Assert.IsType<string>(instance.Endpoint);
            Assert.IsType<string>(instance.EventUpdateMessage);
            Assert.IsType<string>(instance.EventUpdateStatus);
            Assert.IsType<string>(instance.EventUpdateTime);
            Assert.IsType<string>(instance.EventUpdateUser);
            Assert.IsType<string>(instance.EventValue);
            Assert.IsType<string>(instance.HostIp);
            Assert.IsType<string>(instance.HostName);
            Assert.IsType<string>(instance.ItemId);
            Assert.IsType<string>(instance.TriggerDescription);
            Assert.IsType<string>(instance.TriggerId);
            Assert.IsType<string>(instance.ZabbixUrl);
            Assert.IsType<bool>(instance.InAnalisys);
        }

        [Fact]
        public void Should_Validate_IsAcknowledged_From_EventUpdateAction()
        {
            var instance = new EventZabbixParams() { EventUpdateAction = "test" };
                
            Assert.NotNull(instance);
            Assert.False(instance.IsAcknowledged);

            instance.EventUpdateAction = "acknowledged";

            Assert.True(instance.IsAcknowledged);
        }
    }
}
