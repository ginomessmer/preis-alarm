using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Modules
{
    public class UsersModule : ModuleBase<SocketCommandContext>
    {
        private readonly LiteDatabase _liteDatabase;
        public ILiteCollection<FavoriteKeyword> FavoriteKeywords => _liteDatabase.GetCollection<FavoriteKeyword>();

        public UsersModule(LiteDatabase liteDatabase)
        {
            _liteDatabase = liteDatabase;
        }

        [Command("keywords")]
        [Alias("kw")]
        public async Task ListAllFavoriteKeywordsCommandAsync(SocketGuildUser user = null)
        {
            var keywords = FavoriteKeywords.FindAll();
            if (!keywords.Any())
            {
                await ReplyAsync("No keywords found. Add new ones with `kw+`.");
                return;
            }

            if (user != null)
                keywords = keywords.Where(x => x.Creator == user.Id.ToString());

            await ReplyAsync(string.Join(", ", keywords.Select(x => $"`{x.Text}`")));
        }

        [Command("keywords add")]
        [Alias("kw+")]
        public async Task AddFavoriteKeywordCommandAsync(params string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                FavoriteKeywords.Insert(new FavoriteKeyword
                {
                    Text = keyword,
                    Creator = Context.User.Id.ToString()
                });
            }

            await ReplyAsync($"Done.");
        }

        [Command("keywords remove")]
        [Alias("kw-")]
        public async Task RemoveFavoriteKeywordCommandAsync(params string[] keywords)
        {
            FavoriteKeywords.DeleteMany(x => keywords.Contains(x.Text));
            await ReplyAsync($"Done.");
        }
    }
}