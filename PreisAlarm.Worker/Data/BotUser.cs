namespace PreisAlarm.Worker.Data
{
    public class BotUser
    {
        public string Id { get; set; }

        public string EdekaMarketId { get; set; }

        public BotUser(string id)
        {
            Id = id;
        }

        public BotUser()
        {
        }
    }
}