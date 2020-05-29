using NServiceBus;

namespace Messages
{
    public class OrderRequest : OrderMessage, ICommand
    {
    }
}
