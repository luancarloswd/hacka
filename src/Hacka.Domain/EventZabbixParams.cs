namespace Hacka.Domain
{
    public class EventZabbixParams
    {
        public string AlertMessage { get; set; }
        public string AlertSubject { get; set; }
        public string EventDate { get; set; }
        public string EventId { get; set; }
        public string EventNseverity { get; set; }
        public string EventOpdata { get; set; }
        public string EventRecoveryDate { get; set; }
        public string EventRecoveryTime { get; set; }
        public string EventSeverity { get; set; }
        public string EventSource { get; set; }
        public string EventStatus { get; set; }
        public string EventTags { get; set; }
        public string EventTime { get; set; }
        public string EventUpdateAction { get; set; }
        public string EventUpdateDate { get; set; }
        public string Endpoint { get; set; }
        public string EventUpdateMessage { get; set; }
        public string EventUpdateStatus { get; set; }
        public string EventUpdateTime { get; set; }
        public string EventUpdateUser { get; set; }
        public string EventValue { get; set; }
        public string HostIp { get; set; }
        public string HostName { get; set; }
        public string ItemId { get; set; }
        public string TriggerDescription { get; set; }
        public string TriggerId { get; set; }
        public string ZabbixUrl { get; set; }
    }
}
