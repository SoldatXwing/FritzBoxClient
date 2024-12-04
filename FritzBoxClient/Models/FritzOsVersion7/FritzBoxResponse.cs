using Newtonsoft.Json;

namespace FritzBoxClient.Models.FritzOsVersion7
{
    public class FritzBoxResponse
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
