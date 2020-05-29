using System.Collections.Generic;
using NServiceBus;

namespace TravelApp
{
    public class TourSagaData : ContainSagaData
    {
        public string TripId { get; set; }
        public bool SagaIsCanceled { get; set; }
        public Dictionary<OrderType, OrderStatus> OrderStatuses { get; set; } = new Dictionary<OrderType, OrderStatus>() {
            {OrderType.Hotel, OrderStatus.NotOrderedYet},
            {OrderType.Car, OrderStatus.NotOrderedYet},
            {OrderType.FlichtTickets, OrderStatus.NotOrderedYet}
        };
    }
}
