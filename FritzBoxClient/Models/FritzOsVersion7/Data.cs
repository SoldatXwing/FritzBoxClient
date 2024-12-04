using Newtonsoft.Json;

namespace FritzBoxClient.Models.FritzOsVersion7
{
    public class Data
    {
        [JsonProperty("net")]
        public Net Net { get; set; }
    }
}
