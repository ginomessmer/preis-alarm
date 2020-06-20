using System;
using System.Linq;
using System.Web;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PreisAlarm.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => { builder.AddUserSecrets(typeof(Program).Assembly); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<LiteDatabase>(x => new LiteDatabase("data.db"));
                    services.AddSingleton<EdekaReader>();

                    services.AddSingleton<DiscordSocketClient>(x => new DiscordSocketClient());

                    services.AddHostedService<Worker>();
                });
    }
}
