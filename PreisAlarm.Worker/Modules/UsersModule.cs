using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using PreisAlarm.Worker.Data;
using PreisAlarm.Worker.Services;

namespace PreisAlarm.Worker.Modules
{
    public class UsersModule : ModuleBase<SocketCommandContext>
    {
        private readonly IUserService _userService;

        public UsersModule(IUserService userService)
        {
            _userService = userService;
        }

        [Command("keywords")]
        [Alias("kw")]
        public async Task ListAllFavoriteKeywordsCommandAsync(SocketGuildUser user)
        {
            var botUser = await _userService.GetUserAsync(user.Id.ToString());
            var keywords = botUser.FavoriteKeywords;

            if (!keywords.Any())
            {
                await ReplyAsync("Es sind bisher noch keine Stichworte vorhanden. Füge welche mit `kw+` hinzu.");
                return;
            }

            await ReplyAsync(string.Join(", ", keywords.Select(x => $"`{x.Text}`")));
        }

        [Command("keywords add")]
        [Alias("kw+")]
        public async Task AddFavoriteKeywordCommandAsync(params string[] keywords)
        {
            var botUser = await _userService.GetUserAsync(Context.User.Id.ToString());
            foreach (var keyword in keywords)
            {
                botUser.FavoriteKeywords.Add(new FavoriteKeyword
                {
                    Text = keyword,
                    Creator = Context.User.Id.ToString()
                });
            }

            await _userService.UpdateUserAsync(botUser);

            await ReplyAsync($"Fertig.");
        }

        [Command("keywords remove")]
        [Alias("kw-")]
        public async Task RemoveFavoriteKeywordCommandAsync(params string[] keywords)
        {
            var botUser = await _userService.GetUserAsync(Context.User.Id.ToString());
            botUser.FavoriteKeywords.ToList().RemoveAll(x => keywords.Contains(x.Text));
            await _userService.UpdateUserAsync(botUser);

            await ReplyAsync($"Fertig.");
        }
    }
}