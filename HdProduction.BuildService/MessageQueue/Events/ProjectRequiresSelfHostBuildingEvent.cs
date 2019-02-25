using HdProduction.MessageQueue.RabbitMq.Events;

namespace HdProduction.BuildService.MessageQueue.Events
{
    public class ProjectRequiresSelfHostBuildingEvent : HdEvent
    {
        public long ProjectId { get; set; }
        public int SelfHostConfiguration { get; set; }
    }
}