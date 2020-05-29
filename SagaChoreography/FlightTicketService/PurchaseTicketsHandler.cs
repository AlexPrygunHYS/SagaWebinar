using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace FlightTicketService
{
    public class PurchaseTicketsHandler :
        IHandleMessages<RentCarEvent> 
    {
        private static readonly ILog Log = LogManager.GetLogger<PurchaseTicketsHandler>();
        public async Task Handle(RentCarEvent message, IMessageHandlerContext context)
        {
            if(!message.IsBooked) return;
            var result = new TicketsPurchaseEvent { TripId = message.TripId};
            try
            {
                Log.Info($"Tickets for Trip Id {message.TripId} are purchased");
                result.IsBooked = true;
                throw new OperationCanceledException("External service declined your request");
            }
            catch (Exception e)
            {
                Log.Error($"Purchasing tickets for Trip Id {message.TripId} is failed", e);
                result.IsBooked = false;
            }

            await context.Publish(result);
        }
    }
}