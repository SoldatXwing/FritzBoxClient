using Newtonsoft.Json;

namespace FritzBoxClient.Models
{
    public class FritzBoxResponse
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
