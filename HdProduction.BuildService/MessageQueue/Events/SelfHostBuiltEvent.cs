using HdProduction.MessageQueue.RabbitMq.Events;

namespace HdProduction.BuildService.MessageQueue.Events
{
    public class SelfHostBuiltEvent : HdEvent
    {
        public long ProjectId { get; set; }
        public string DownloadLink { get; set; }
    }
}