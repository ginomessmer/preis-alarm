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
using PreisAlarm.Worker.Readers;
using PreisAlarm.Worker.Services;

namespace PreisAlarm.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EdekaReader _edekaReader;
        private readonly LiteDatabase _liteDatabase;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandHandler _commandHandler;
        private readonly IConfiguration _configuration;

        public const string PforzheimMarketId = "1160950";
        public IEnumerable<FavoriteKeyword> FavoriteKeywords =>
            _liteDatabase.GetCollection<FavoriteKeyword>().FindAll();

        public Worker(ILogger<Worker> logger,
            EdekaReader edekaReader,
            LiteDatabase liteDatabase,
            DiscordSocketClient discordSocketClient,
            CommandHandler commandHandler,
            IConfiguration configuration)
        {
            _logger = logger;
            _edekaReader = edekaReader;
            _liteDatabase = liteDatabase;
            _discordSocketClient = discordSocketClient;
            _commandHandler = commandHandler;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartDiscordClientAsync();
            await _commandHandler.InitializeAsync();
        }

        private async Task StartDiscordClientAsync()
        {
            await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration.GetConnectionString("DiscordBotToken"));
            await _discordSocketClient.StartAsync();

            _discordSocketClient.Ready += () =>
            {
                //await _discordSocketClient.SetActivityAsync(new Game( "Sieben sieben ay lulu, eins zwei",
                //    ActivityType.Listening));
            };
        }
    }
}
