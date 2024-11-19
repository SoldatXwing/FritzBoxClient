using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FritzBoxClient.Models
{
    public class Port
    {
        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("port")]
        public int PortNumber { get; set; }

        [JsonProperty("protocols")]
        [JsonConverter(typeof(ProtocolConverter))]
        public List<Protocol> UsedProtocols { get; set; }

        [JsonProperty("idx")]
        public int Idx { get; set; }
    }
    public enum Protocol
    {
        UdpIPv4,
        UdpIPv6,
        UdpBoth,
        TcpIPv4,
        TcpIPv6,
        TcpBoth
    }

    public class ProtocolConverter : JsonConverter<List<Protocol>>
    {
        public override List<Protocol>? ReadJson(JsonReader reader, Type objectType, List<Protocol>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var protocolStr = reader.Value!.ToString();
                return new List<Protocol>
            {
                protocolStr switch
                {
                    "UDP (IPv4)" => Protocol.UdpIPv4,
                    "UDP (IPv6)" => Protocol.UdpIPv6,
                    "UDP (IPv4/v6)" => Protocol.UdpBoth,
                    "TCP (IPv4)" => Protocol.TcpIPv4,
                    "TCP (IPv6)" => Protocol.TcpIPv6,
                    "TCP (IPv4/v6)" => Protocol.TcpBoth,
                    _ => throw new JsonSerializationException($"Unknown protocol: {protocolStr}")
                }
            };
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                var protocols = new List<Protocol>();
                var array = JArray.Load(reader);
                foreach (var item in array)
                {
                    var protocolStr = item.ToString();
                    protocols.Add(protocolStr switch
                    {
                        "UDP (IPv4)" => Protocol.UdpIPv4,
                        "UDP (IPv6)" => Protocol.UdpIPv6,
                        "UDP (IPv4/v6)" => Protocol.UdpBoth,
                        "TCP (IPv4)" => Protocol.TcpIPv4,
                        "TCP (IPv6)" => Protocol.TcpIPv6,
                        "TCP (IPv4/v6)" => Protocol.TcpBoth,
                        _ => throw new JsonSerializationException($"Unknown protocol: {protocolStr}")
                    });
                }
                return protocols;
            }

            throw new JsonSerializationException("Unexpected token when parsing protocols");
        }

        public override void WriteJson(JsonWriter writer, List<Protocol>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Only deserialization is implemented for ProtocolConverter.");
        }
    }

}
