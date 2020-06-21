using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PreisAlarm.Worker.Data;

namespace PreisAlarm.Worker
{
    public class EdekaReader
    {
        private HttpClient _client;
        public const string EdekaOffersEndpoint = "https://www.edeka.de/eh/service/eh/offers";
        public const string EdekaMarketsEndpoint = "https://www.edeka.de/api/marketsearch/markets";

        public EdekaReader()
        {
            _client = new HttpClient();
        }

        public async Task<List<EdekaMarket>> GetNearbyMarketsAsync(string term)
        {
            var url = $"{EdekaMarketsEndpoint}?searchstring={term}";
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var marketsJson = JsonDocument.Parse(json).RootElement.GetProperty("markets").GetRawText();
            var markets = JsonConvert.DeserializeObject<List<EdekaMarket>>(marketsJson);

            return markets;
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