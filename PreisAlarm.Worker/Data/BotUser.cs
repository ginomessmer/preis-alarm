using System.Collections.Generic;

namespace PreisAlarm.Worker.Data
{
    public class BotUser
    {
        public string Id { get; set; }

        public string PreferredEdekaMarketId { get; set; }

        public ICollection<FavoriteKeyword> FavoriteKeywords { get; set; } = new List<FavoriteKeyword>();

        public BotUser(string id)
        {
            Id = id;
        }

        public BotUser()
        {
        }
    }
}