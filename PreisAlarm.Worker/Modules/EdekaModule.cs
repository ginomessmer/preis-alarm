using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LiteDB;
using PreisAlarm.Worker.Data;
using PreisAlarm.Worker.Readers;
using PreisAlarm.Worker.Services;

namespace PreisAlarm.Worker.Modules
{
    [Group("edeka"), Alias("edk")]
    public class EdekaModule : ModuleBase<SocketCommandContext>
    {
        private readonly EdekaReader _edekaReader;
        private readonly IUserService _userService;

        public EdekaModule(EdekaReader edekaReader, IUserService userService)
        {
            _edekaReader = edekaReader;
            _userService = userService;
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
            var botUser = await _userService.GetUserAsync(Context.User.Id.ToString());

            botUser.PreferredEdekaMarketId = marketId;

            await _userService.UpdateUserAsync(botUser);
            await ReplyAsync("Änderung übernommen.");
        }

        [Command("deals"), Alias("d")]
        public async Task CurrentDealsCommandAsync()
        {
            var botUser = await _userService.GetUserAsync(Context.User.Id.ToString());

            if (string.IsNullOrEmpty(botUser.PreferredEdekaMarketId))
            {
                await ReplyAsync("Du musst zuerst deinen präferierten Edeka Markt setzen. " +
                                 "Suche dazu mit `edeka markets <suchbegriff>` deinen Lieblingsmarke " +
                                 "und setze diesen dann mit `edeka set marketId <id>` fest.");
                return;
            }

            if (!botUser.FavoriteKeywords.Any())
            {
                await ReplyAsync("Bevor ich deine Angebote finden kann, " +
                                 "musst du mir ein paar Stichworte zu deinen Lieblingsprodukten " +
                                 "eingeben: `kw+ <stichwort1> [<stichwort2> ...]`. Versuche es dann erneut.");
                return;
            }

            var deals = await _edekaReader.GetCurrentDealsAsync(botUser.PreferredEdekaMarketId);
            var favoriteDeals = deals
                .Where(x => botUser.FavoriteKeywords
                    .Any(y => x.Title.Contains(y.Text)))
                .OrderBy(x => x.Price)
                .ToList();

            if (!favoriteDeals.Any())
            {
                await ReplyAsync("Ich konnte keine Angebote finden :(");
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
