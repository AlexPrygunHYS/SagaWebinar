using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace RentCarService
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        static async Task Main(string[] args)
        {
            Console.Title = "RentCarService";

            var endpointConfiguration = new EndpointConfiguration("RentCarService");

            endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            Console.ReadKey();

            await endpointInstance.Stop();

        }
    }
}
