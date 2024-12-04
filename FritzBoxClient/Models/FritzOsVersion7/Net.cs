using Newtonsoft.Json;

namespace FritzBoxClient.Models.FritzOsVersion7
{
    public class Net
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }
    }
}