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
            await ReplyAsync("Changed saved.");
        }

        [Command("deals")]
        public async Task CurrentDealsCommandAsync()
        {
            var user = BotUsers.FindById(Context.User.Id.ToString());

            if (user is null)
            {
                await ReplyAsync("You need to set your preferred market ID first: `set marketId <id>`");
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
