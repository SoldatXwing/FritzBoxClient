using Newtonsoft.Json;
using System.Net;

namespace FritzBoxClient.Converter
{
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress? value, JsonSerializer serializer) => writer.WriteValue(value?.ToString());

        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress? existingValue, bool hasExistingValue, JsonSerializer serializer) => string.IsNullOrEmpty((string)reader.Value!)! ? null! : IPAddress.Parse((string)reader.Value!)!;

    }
}