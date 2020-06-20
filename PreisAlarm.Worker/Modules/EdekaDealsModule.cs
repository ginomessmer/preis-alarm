using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LiteDB;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Modules
{
    public class EdekaDealsModule : ModuleBase<SocketCommandContext>
    {
        private readonly EdekaReader _edekaReader;
        private readonly LiteDatabase _liteDatabase;

        public IEnumerable<FavoriteKeyword> FavoriteKeywords =>
            _liteDatabase.GetCollection<FavoriteKeyword>().FindAll();

        public EdekaDealsModule(EdekaReader edekaReader, LiteDatabase liteDatabase)
        {
            _edekaReader = edekaReader;
            _liteDatabase = liteDatabase;
        }

        [Command("deals")]
        public async Task CurrentDealsCommandAsync(string marketId = "1160950")
        {
            var deals = await _edekaReader.GetCurrentDealsAsync(marketId);
            var favoriteDeals = deals.Where(x => FavoriteKeywords.Any(y => x.Title.Contains(y.Text))).ToList();

            foreach (var deal in favoriteDeals)
            {
                var embed = new EmbedBuilder()
                    .WithTitle(deal.Title)
                    .WithDescription(deal.Description)
                    .WithThumbnailUrl(deal.BildApp.ToString())
                    .WithColor(Color.LightOrange)
                    .WithFields(new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder()
                            .WithName("Price")
                            .WithValue(deal.Price)
                            .WithIsInline(true),
                        new EmbedFieldBuilder()
                            .WithName("Basic Price")
                            .WithValue(deal.BasicPrice)
                            .WithIsInline(true),
                        new EmbedFieldBuilder()
                            .WithName("Category")
                            .WithValue(deal.Category)
                    })
                    .WithCurrentTimestamp()
                    .Build();

                await ReplyAsync(string.Empty, embed: embed);
            }
        }
    }
}
