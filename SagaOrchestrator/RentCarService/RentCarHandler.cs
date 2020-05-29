using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace RentCarService
{
    public class RentCarHandler :
        IHandleMessages<RentCarRequest>,
        IHandleMessages<CancelCarRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger<RentCarHandler>();

        public async Task Handle(RentCarRequest message, IMessageHandlerContext context)
        {
            var result = new RentCarResponse { TripId = message.TripId};
            try
            {
                Log.Info($"Car for Trip Id {message.TripId} is rent");
                result.IsBooked = true;
            }
            catch (Exception e)
            {
                Log.Info($"Booking car for Trip Id {message.TripId} is failed", e);
                result.IsBooked = false;
            }
            
            await context.Publish(result);
        }

        public async Task Handle(CancelCarRequest message, IMessageHandlerContext context)
        {
            Log.Info($"Car for Trip Id {message.TripId} is canceled");
            var result = new RentCarResponse { TripId = message.TripId, IsBooked = false };
            await context.Publish(result);
        }
    }
}