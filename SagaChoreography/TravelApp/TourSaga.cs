using System.Linq;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace TravelApp
{
    public class TourSaga : Saga<TourSagaData>,
        IAmStartedByMessages<NewOrderEvent>,
        IAmStartedByMessages<BookHotelEvent>,
        IAmStartedByMessages<RentCarEvent>,
        IAmStartedByMessages<TicketsPurchaseEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<TourSaga>();
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TourSagaData> mapper)
        {
            mapper.ConfigureMapping<OrderEvent>(message => message.TripId)
                .ToSaga(sagaData => sagaData.TripId);
        }

        public Task Handle(NewOrderEvent message, IMessageHandlerContext context)
        {
            Log.Info($"Tour booking started. Tour Id {message.TripId} ");
            return Task.CompletedTask;
        }

        public Task Handle(BookHotelEvent message, IMessageHandlerContext context)
        {
            if (message.IsBooked)
            {
                Data.OrderStatuses[OrderType.Hotel] = OrderStatus.Confirmed;
                Log.Info($"Hotel booking confirmation received for Tour Id {message.TripId}");
            }
            else
            {
                Log.Info($"Hotel is canceled for Tour Id {message.TripId}");
                Data.OrderStatuses[OrderType.Hotel] = OrderStatus.Canceled;
            }

            TryCompleteSaga();
            return Task.CompletedTask;
        }

        public Task Handle(RentCarEvent message, IMessageHandlerContext context)
        {
            if (message.IsBooked)
            {
                Data.OrderStatuses[OrderType.Car] = OrderStatus.Confirmed;
                Log.Info($"Car is rented for Tour Id {message.TripId}");
            }
            else
            {
                Log.Info($"Car is canceled for Tour Id {message.TripId}");
                Data.OrderStatuses[OrderType.Car] = OrderStatus.Canceled;
            }

            TryCompleteSaga();
            return Task.CompletedTask;
        }

        public Task Handle(TicketsPurchaseEvent message, IMessageHandlerContext context)
        {
            if (message.IsBooked)
            {
                Data.OrderStatuses[OrderType.FlichtTickets] = OrderStatus.Confirmed;
                Log.Info($"Tickets are purchased for Tour Id {message.TripId} is confirmed");
            }
            else
            {
                Log.Info($"Tickets are canceled for Tour Id {message.TripId}");
                Data.OrderStatuses[OrderType.FlichtTickets] = OrderStatus.Canceled;
            }
            TryCompleteSaga();
            return Task.CompletedTask;
        }

        private void TryCompleteSaga()
        {
            if (Data.OrderStatuses.Values.All(x => x == OrderStatus.Confirmed))
            {
                MarkAsComplete();
                Log.Info($"Tour purchase is completed successfully. Trip Id {Data.TripId}");
                return;
            }

            if (Data.OrderStatuses.Values.All(x => x == OrderStatus.Canceled || x == OrderStatus.NotOrderedYet))
            {
                Log.Info($"Tour purchase was canceled. Trip Id {Data.TripId}");
                Data.SagaIsCanceled = true;
                MarkAsComplete();
            }
        }
    }
}
