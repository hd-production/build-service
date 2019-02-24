using HdProduction.MessageQueue.RabbitMq.Events;

namespace HdProduction.BuildService.MessageQueue.Events
{
    public class SelfHostBuildingFailedEvent : HdEvent
    {
        public long ProjectId { get; set; }
        public string Exception { get; set; }
    }
}