using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace HotelBookingService
{
    internal class BookHotelHandler : 
        IHandleMessages<NewOrderEvent>,
        IHandleMessages<RentCarEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<BookHotelHandler>();
        public async Task Handle(NewOrderEvent message, IMessageHandlerContext context)
        {
            var result = new BookHotelEvent { TripId = message.TripId};

            try
            {
                Log.Info($"Hotel for Trip Id {message.TripId} is booked");
                result.IsBooked = true;
            }
            catch (Exception e)
            {
                Log.Error($"Hotel booking failed for Trip Id {message.TripId}", e);
                result.IsBooked = false;
            }

            await context.Publish(result);
        }

        public async Task Handle(RentCarEvent message, IMessageHandlerContext context)
        {
            if(message.IsBooked) return;
            Log.Info($"Hotel for Trip Id {message.TripId} is canceled");
            var result = new BookHotelEvent { TripId = message.TripId, IsBooked = false};
            await context.Publish(result);
        }
    }
}
