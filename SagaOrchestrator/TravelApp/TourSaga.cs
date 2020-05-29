using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace TravelApp
{
    public class TourSaga : Saga<TourSagaData>,
        IAmStartedByMessages<TourRequest>,
        IHandleMessages<BookingHotelResponse>,
        IHandleMessages<RentCarResponse>,
        IHandleMessages<TicketsPurchaseResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger<TourSaga>();
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TourSagaData> mapper)
        {
            mapper.ConfigureMapping<OrderRequest>(message => message.TripId)
                .ToSaga(sagaData => sagaData.TripId);
            mapper.ConfigureMapping<OrderResponse>(message => message.TripId)
                .ToSaga(sagaData => sagaData.TripId);
        }

        public async Task Handle(TourRequest message, IMessageHandlerContext context)
        {
            Log.Info("Sending book hotel request...");
            var command = new BookHotelRequest {TripId = message.TripId};
            await context.Send(command);
            Data.OrderStatuses[OrderType.Hotel] = OrderStatus.OrderRequestIsSent;
        }

        public async Task Handle(BookingHotelResponse message, IMessageHandlerContext context)
        {
            if (message.IsBooked)
            {
                Data.OrderStatuses[OrderType.Hotel] = OrderStatus.Confirmed;
                Log.Info($"Hotel booking confirmation received for Tour Id {message.TripId}");
                Log.Info("Sending rent car request...");
                var command = new RentCarRequest {TripId = message.TripId};
                await context.Send(command);
                Data.OrderStatuses[OrderType.Car] = OrderStatus.OrderRequestIsSent;
            }
            else
            {
                Log.Info($"Hotel is canceled for Tour Id {message.TripId}");
                Data.OrderStatuses[OrderType.Hotel] = OrderStatus.Canceled;
                await CancelSaga(context);
            }

            await TryCompleteSaga();
        }

        public async Task Handle(RentCarResponse message, IMessageHandlerContext context)
        {
            if (message.IsBooked)
            {
                Data.OrderStatuses[OrderType.Car] = OrderStatus.Confirmed;
                Log.Info($"Car is rented for Tour Id {message.TripId}");
                Log.Info("Sending purchase tickets request...");
                var command = new TicketsPurchaseRequest {TripId = message.TripId};
                await context.Send(command);
                Data.OrderStatuses[OrderType.FlichtTickets] = OrderStatus.OrderRequestIsSent;
            }
            else
            {
                Log.Info($"Car is canceled for Tour Id {message.TripId}");
                Data.OrderStatuses[OrderType.Car] = OrderStatus.Canceled;
                await CancelSaga(context);
            }

            await TryCompleteSaga();
        }

        public async Task Handle(TicketsPurchaseResponse message, IMessageHandlerContext context)
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
                await CancelSaga(context);
            }
            await TryCompleteSaga();
        }

        private async Task TryCompleteSaga()
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

        private async Task CancelSaga(IMessageHandlerContext context)
        {
            var orders = Data.OrderStatuses.Where(x => x.Value == OrderStatus.Confirmed).ToList();
            foreach (var order in orders)
            {
                switch (order.Key)
                {
                    case OrderType.Hotel:
                        await CancelHotel(context);
                        break;
                    case OrderType.Car:
                        await CancelCar(context);
                        break;
                    case OrderType.FlichtTickets:
                        await CancelFlightTickets(context);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task CancelHotel(IMessageHandlerContext context)
        {
            Log.Info($"Sending cancel hotel request for Trip Id {Data.TripId}");
            Data.OrderStatuses[OrderType.Hotel] = OrderStatus.CancelRequestIsSent;
            var command = new CancelHotelRequest {TripId = Data.TripId};
            await context.Send(command);

        }

        private async Task CancelCar(IMessageHandlerContext context)
        {
            Log.Info($"Sending cancel car request for Trip Id {Data.TripId}");
            Data.OrderStatuses[OrderType.Car] = OrderStatus.CancelRequestIsSent;
            var command = new CancelCarRequest { TripId = Data.TripId };
            await context.Send(command);
        }

        private async Task CancelFlightTickets(IMessageHandlerContext context)
        {
            Log.Info($"Sending cancel tickets request for Trip Id {Data.TripId}");
            Data.OrderStatuses[OrderType.FlichtTickets] = OrderStatus.CancelRequestIsSent;
            var command = new CancelTicketsRequest { TripId = Data.TripId };
            await context.Send(command);
        }
    }
}
