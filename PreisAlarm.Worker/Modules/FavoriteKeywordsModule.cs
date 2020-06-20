using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LiteDB;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Modules
{
    public class FavoriteKeywordsModule : ModuleBase<SocketCommandContext>
    {
        private readonly LiteDatabase _liteDatabase;
        public ILiteCollection<FavoriteKeyword> FavoriteKeywords => _liteDatabase.GetCollection<FavoriteKeyword>();

        public FavoriteKeywordsModule(LiteDatabase liteDatabase)
        {
            _liteDatabase = liteDatabase;
        }

        [Command("keywords")]
        [Alias("kw")]
        public async Task ListAllFavoriteKeywordsCommandAsync()
        {
            var keywords = FavoriteKeywords.FindAll();
            await ReplyAsync(string.Join(", ", keywords.Select(x => $"`{x.Text}`")));
        }

        [Command("keywords add")]
        [Alias("kw+")]
        public async Task AddFavoriteKeywordCommandAsync(string text)
        {
            FavoriteKeywords.Insert(new FavoriteKeyword
            {
                Text = text,
                Creator = Context.User.Id.ToString()
            });

            await ReplyAsync($"`{text}` added");
        }

        [Command("keywords remove")]
        [Alias("kw-")]
        public async Task RemoveFavoriteKeywordCommandAsync(string text)
        {
            FavoriteKeywords.DeleteMany(x => x.Text == text);
            await ReplyAsync($"`{text}` removed");
        }
    }
}