using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace HotelBookingService
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            Console.Title = "HotelBookingService";
            
            var endpointConfiguration = new EndpointConfiguration("HotelBookingService");

            endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            Console.ReadKey();

            await endpointInstance.Stop();

        }
    }
}
