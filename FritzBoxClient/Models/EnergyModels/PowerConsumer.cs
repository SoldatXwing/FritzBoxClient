using FritzBoxClient.Converter;
using Newtonsoft.Json;

namespace FritzBoxClient.Models.EnergyModels
{
    public class PowerConsumer
    {
        [JsonProperty("cumPerc")]
        public int CumulativeLoadPercentage { get; set; } = default;
        [JsonProperty("actPerc")]
        public int CurrentLoadPercentage { get; set; } = default;
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("statuses")]
        [JsonConverter(typeof(PowerStatusConverter))]
        public List<string> Statuses { get; set; }
        [JsonProperty("lan")]
        public List<Lan>? Lans { get; set; } = null;
    }
    public class Lan
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("class")]
        public string Class { get; set; }
    }
}
