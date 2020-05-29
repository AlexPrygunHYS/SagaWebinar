using NServiceBus;

namespace Messages
{
    public class OrderEvent : OrderMessage, IEvent
    {
        public bool IsBooked { get; set; }
    }
}
