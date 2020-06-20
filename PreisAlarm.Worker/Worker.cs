using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EdekaReader _edekaReader;
        private readonly LiteDatabase _liteDatabase;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;

        public const string PforzheimMarketId = "1160950";
        public IEnumerable<FavoriteKeyword> FavoriteKeywords =>
            _liteDatabase.GetCollection<FavoriteKeyword>().FindAll();

        public Worker(ILogger<Worker> logger,
            EdekaReader edekaReader,
            LiteDatabase liteDatabase,
            DiscordSocketClient discordSocketClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _edekaReader = edekaReader;
            _liteDatabase = liteDatabase;
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDiscordClientAsync();
            SeedKeywords();
                
            while (!stoppingToken.IsCancellationRequested)
            {
                var deals = await _edekaReader.GetCurrentDealsAsync(PforzheimMarketId);
                deals = deals.Where(x => FavoriteKeywords.Any(y => x.Title.Contains(y.Text))).ToList();

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

        private async Task StartDiscordClientAsync()
        {
            await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration.GetConnectionString("DiscordBotToken"));
            await _discordSocketClient.StartAsync();

            _discordSocketClient.Ready += async () =>
            {
                await _discordSocketClient.SetActivityAsync(new Game( "Sieben sieben ay lulu, eins zwei",
                    ActivityType.Listening));
            };
        }

        private void SeedKeywords()
        {
            var existsAnyFavoriteKeywords = _liteDatabase.GetCollection<FavoriteKeyword>().Count() > 0;

            if (existsAnyFavoriteKeywords)
                return;
            
            _liteDatabase.GetCollection<FavoriteKeyword>().InsertBulk(new List<FavoriteKeyword>
            {
                new FavoriteKeyword {Text = "Nüsse"},
                new FavoriteKeyword {Text = "Eis"},
                new FavoriteKeyword {Text = "TropiFrutti"},
                new FavoriteKeyword {Text = "Studentenfutter"}
            });
        }
    }
}
