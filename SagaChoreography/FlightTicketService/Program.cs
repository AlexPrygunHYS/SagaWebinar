using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace FlightTicketService
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            Console.Title = "FlightTicketsService";

            var endpointConfiguration = new EndpointConfiguration("FlightTicketsService");

            endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            Console.ReadKey();

            await endpointInstance.Stop();

        }
    }
}
