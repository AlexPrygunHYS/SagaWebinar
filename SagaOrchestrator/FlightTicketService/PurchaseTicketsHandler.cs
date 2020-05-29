using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace FlightTicketService
{
    public class PurchaseTicketsHandler :
        IHandleMessages<TicketsPurchaseRequest>,
        IHandleMessages<CancelTicketsRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger<PurchaseTicketsHandler>();
        public async Task Handle(TicketsPurchaseRequest message, IMessageHandlerContext context)
        {
            var result = new TicketsPurchaseResponse { TripId = message.TripId};
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

        public async Task Handle(CancelTicketsRequest message, IMessageHandlerContext context)
        {
            Log.Info($"Tickets for Trip Id {message.TripId} are canceled");
            var result = new TicketsPurchaseResponse { TripId = message.TripId, IsBooked = false};
            await context.Publish(result);
        }
    }
}