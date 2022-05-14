using Newtonsoft.Json;

namespace TheaterDaysScore.JsonModels {

    // http://mirishitadb.php.xdomain.jp/db/#profile (id modified)
    public class IdolData {
        public int id { get; set; }
        public string name { get; set; }

        [JsonProperty(PropertyName = "class")]
        public string classField { get; set; }
        public string colour { get; set; }
    }
}
