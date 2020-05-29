using NServiceBus;

namespace Messages
{
    public class OrderResponse : OrderMessage, IEvent
    {
        public bool IsBooked { get; set; }
    }
}
