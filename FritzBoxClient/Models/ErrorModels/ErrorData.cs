using Newtonsoft.Json;

namespace FritzBoxClient.Models.ErrorModels
{
    public class ErrorData
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

}
