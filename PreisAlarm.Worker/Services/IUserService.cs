using System.Threading.Tasks;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Services
{
    public interface IUserService
    {
        Task<BotUser> GetUser(string id);

        Task UpdateUser(BotUser user);
    }
}