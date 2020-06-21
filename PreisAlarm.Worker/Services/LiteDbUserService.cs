using System.Threading.Tasks;
using LiteDB;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Services
{
    public class LiteDbUserService : IUserService
    {
        private readonly LiteDatabase _liteDatabase;

        private ILiteCollection<BotUser> BotUserCollection => _liteDatabase.GetCollection<BotUser>();

        public LiteDbUserService(LiteDatabase liteDatabase)
        {
            _liteDatabase = liteDatabase;
        }

        public Task<BotUser> GetUserAsync(string id)
        {
            var usersExists = BotUserCollection.Exists(x => x.Id == id);

            if (!usersExists)
                BotUserCollection.Insert(new BotUser(id));

            var user = BotUserCollection.FindById(id);
            return Task.FromResult(user);
        }

        public Task UpdateUserAsync(BotUser user) => Task.FromResult(BotUserCollection.Update(user));
    }
}