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

        public Task<BotUser> GetUser(string id) => Task.FromResult(BotUserCollection.FindById(id));

        public Task UpdateUser(BotUser user) => Task.FromResult(BotUserCollection.Update(user));
    }
}