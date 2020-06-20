using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PreisAlarm.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EdekaReader _edekaReader;

        public const string pforzheimMarketId = "1160950";

        public Worker(ILogger<Worker> logger, EdekaReader edekaReader)
        {
            _logger = logger;
            _edekaReader = edekaReader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var deals = await _edekaReader.GetCurrentDealsAsync(pforzheimMarketId);

                Console.WriteLine($"Found {deals.Count} deals");
                foreach (var edekaDeal in deals)
                {
                    Console.WriteLine(edekaDeal.Title);
                    Console.WriteLine("Price: {0} - Original Price: {1}", edekaDeal.Price, edekaDeal.BasicPrice);
                    Console.WriteLine("============================");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
