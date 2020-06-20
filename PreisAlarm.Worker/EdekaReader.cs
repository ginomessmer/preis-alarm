using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker
{
    public class EdekaReader
    {
        private HttpClient _client;
        public const string EdekaOffersEndpoint = "https://www.edeka.de/eh/service/eh/offers";

        public EdekaReader()
        {
            _client = new HttpClient();
        }

        public async Task<List<EdekaDeal>> GetCurrentDealsAsync(string marketId)
        {
            var url = $"{EdekaOffersEndpoint}?marketId={marketId}&limit=89899";
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var deals = JsonConvert.DeserializeObject<EdekaOfferDocument>(json);

            return deals.Items;
        }
    }
}