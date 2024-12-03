using Newtonsoft.Json;

namespace FritzBoxClient.Models
{
    public class Net
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }
    }
}