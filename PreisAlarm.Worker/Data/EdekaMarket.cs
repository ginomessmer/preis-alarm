using Newtonsoft.Json;

namespace PreisAlarm.Worker.Data
{
    public class EdekaMarket
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}