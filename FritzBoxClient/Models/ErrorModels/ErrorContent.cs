using FritzBoxClient.Converter;
using Newtonsoft.Json;

namespace FritzBoxClient.Models.ErrorModels
{
    public class ErrorContent
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        [JsonConverter(typeof(ErrorDataConverter))]
        public List<ErrorData> Data { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

}
