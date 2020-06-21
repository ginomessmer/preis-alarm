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
    [Group("edeka"), Alias("edk")]
    public class EdekaModule : ModuleBase<SocketCommandContext>
    {
        private readonly EdekaReader _edekaReader;
        private readonly LiteDatabase _liteDatabase;

        public IEnumerable<FavoriteKeyword> FavoriteKeywords =>
            _liteDatabase.GetCollection<FavoriteKeyword>().FindAll();

        public ILiteCollection<BotUser> BotUsers => _liteDatabase.GetCollection<BotUser>();

        public EdekaModule(EdekaReader edekaReader, LiteDatabase liteDatabase)
        {
            _edekaReader = edekaReader;
            _liteDatabase = liteDatabase;
        }

        [Command("markets"), Alias("m")]
        public async Task FindMarketsCommandAsync([Remainder] string term)
        {
            var markets = await _edekaReader.GetNearbyMarketsAsync(term);

            var embed = new EmbedBuilder()
                .WithTitle("Marktsuche")
                .WithDescription($"Hier ist eine Liste aller Märkte und ihrer ID in der Nähe von `{term}`")
                .WithFields(markets
                    .Select(x => new EmbedFieldBuilder()
                        .WithName(x.Name)
                        .WithValue($"`{x.Id}`")
                        .WithIsInline(true)))
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("set marketId")]
        public async Task SetMarketIdCommandAsync(string marketId)
        {
            var userId = Context.User.Id.ToString();
            var exists = BotUsers.Exists(x => x.Id == userId);

            var user = new BotUser(userId);
            if (exists)
                user = BotUsers.FindById(userId);
            else
                BotUsers.Insert(user);

            user.EdekaMarketId = marketId;

            BotUsers.Update(user);
            await ReplyAsync("Änderung übernommen.");
        }

        [Command("deals"), Alias("d")]
        public async Task CurrentDealsCommandAsync()
        {
            var user = BotUsers.FindById(Context.User.Id.ToString());

            if (user is null)
            {
                await ReplyAsync("Du musst zuerst deinen präferierten Edeka Markt setzen. " +
                                 "Suche dazu mit `edeka markets <suchbegriff>` deinen Lieblingsmarke " +
                                 "und setze diesen dann mit `edeka set marketId <id>` fest.");
                return;
            }

            var deals = await _edekaReader.GetCurrentDealsAsync(user.EdekaMarketId);
            var favoriteDeals = deals
                .Where(x => FavoriteKeywords
                    .Any(y => x.Title.Contains(y.Text) && y.Creator == Context.User.Id.ToString()))
                .OrderBy(x => x.Price)
                .ToList();

            if (!favoriteDeals.Any())
            {
                await ReplyAsync("I didn't found any deals :(");
                return;
            }

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
                            .WithValue(deal.BasicPrice ?? "-")
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
