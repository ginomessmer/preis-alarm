using System.Collections.Generic;
using Newtonsoft.Json;

namespace PreisAlarm.Worker.Data
{
    public class EdekaOfferDocument
    {
        [JsonProperty("docs")]
        public List<EdekaDeal> Items { get; set; }
    }
}