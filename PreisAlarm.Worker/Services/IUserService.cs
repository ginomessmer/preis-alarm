using System.Threading.Tasks;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Gets or creates the user with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BotUser> GetUserAsync(string id);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUserAsync(BotUser user);
    }
}