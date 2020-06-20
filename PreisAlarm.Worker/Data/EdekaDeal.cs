using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PreisAlarm.Worker.Data
{
    public class EdekaDeal
    {
        [JsonProperty("angebotid")]
        public long Id { get; set; }

        [JsonProperty("externeid")]
        public object ExternalId { get; set; }

        [JsonProperty("titel")]
        public string Title { get; set; }

        [JsonProperty("preis")]
        public double Price { get; set; }

        [JsonProperty("beschreibung")]
        public string Description { get; set; }

        [JsonProperty("zusatztext")]
        public string Annotation { get; set; }

        [JsonProperty("bild_web90")]
        public Uri BildWeb90 { get; set; }

        [JsonProperty("bild_web130")]
        public Uri BildWeb130 { get; set; }

        [JsonProperty("bild_app")]
        public Uri BildApp { get; set; }

        [JsonProperty("warengruppe")]
        public string Category { get; set; }

        [JsonProperty("warengruppeid")]
        public long Warengruppeid { get; set; }

        [JsonProperty("sonderkennzeichen")]
        public object SpecialNotices { get; set; }

        [JsonProperty("zutaten")]
        public object Ingredients { get; set; }

        [JsonProperty("nachlass")]
        public object Discount { get; set; }

        [JsonProperty("basicPrice")]
        public object BasicPrice { get; set; }

        [JsonProperty("national")]
        public bool National { get; set; }

        //[JsonProperty("gueltig_bis")]
        //public long ValidUntil { get; set; }
    }
}
