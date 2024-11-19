using Newtonsoft.Json;

namespace FritzBoxClient.Models.ErrorModels
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public ErrorContent Error { get; set; }
    }

}
