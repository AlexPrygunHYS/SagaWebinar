using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;
using Messages;

namespace TravelApp
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            Console.Title = "TravelApp";

            var endpointConfiguration = new EndpointConfiguration("TravelApp");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(BookHotelRequest), "HotelBookingService");
            routing.RouteToEndpoint(typeof(CancelHotelRequest), "HotelBookingService");
            routing.RouteToEndpoint(typeof(RentCarRequest), "RentCarService");
            routing.RouteToEndpoint(typeof(CancelCarRequest), "RentCarService");
            routing.RouteToEndpoint(typeof(TicketsPurchaseRequest), "FlightTicketsService");
            routing.RouteToEndpoint(typeof(CancelTicketsRequest), "FlightTicketsService");

            endpointConfiguration.UsePersistence<LearningPersistence>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            await RunLoop(endpointInstance);

            await endpointInstance.Stop();

        }

        private static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                Log.Info("Press 'T' to order the tour, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.T:
                        var command = new TourRequest { TripId = Guid.NewGuid().ToString()};
                        await endpointInstance.SendLocal(command);
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        Log.Info("Please try again...");
                        break;
                }
            }
        }
    }
}
