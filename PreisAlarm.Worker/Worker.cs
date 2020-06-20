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

        public const string PforzheimMarketId = "1160950";
        public IList<string> FavoriteKeywords = new List<string>
        {
            "Nüsse", "Eis", "TropiFrutti", "Studentenfutter"
        };

        public Worker(ILogger<Worker> logger, EdekaReader edekaReader)
        {
            _logger = logger;
            _edekaReader = edekaReader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var deals = await _edekaReader.GetCurrentDealsAsync(PforzheimMarketId);
                deals = deals.Where(x => FavoriteKeywords.Any(y => x.Title.Contains(y))).ToList();

                Console.WriteLine($"Found {deals.Count} deals");
                foreach (var edekaDeal in deals)
                {
                    Console.WriteLine(edekaDeal.Title);
                    Console.WriteLine("Price: {0} - Basic Price: {1}", edekaDeal.Price, edekaDeal.BasicPrice);
                    Console.WriteLine("============================");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
