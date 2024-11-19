using Newtonsoft.Json;

namespace FritzBoxClient.Models
{
    public class Data
    {
        [JsonProperty("net")]
        public Net Net { get; set; }
    }
}
