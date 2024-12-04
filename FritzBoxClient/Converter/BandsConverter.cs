using FritzBoxClient.Models.FritzOsVersion7;
using Newtonsoft.Json;

namespace FritzBoxClient.Converter
{
    public class BandsConverter : JsonConverter<Dictionary<string, Band>>
    {
        public override Dictionary<string, Band>? ReadJson(JsonReader reader, Type objectType, Dictionary<string, Band>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var result = new Dictionary<string, Band>();

            if (reader.TokenType == JsonToken.StartObject)
            {
                var dictionary = serializer.Deserialize<Dictionary<string, Band>>(reader);
                if (dictionary != null)
                {
                    foreach (var kvp in dictionary)
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                var list = serializer.Deserialize<List<Band>>(reader);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        result.Add(i.ToString(), list[i]);
                    }
                }
            }
            else
            {
                throw new JsonSerializationException("Unexpected token type for 'bands'.");
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<string, Band>? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Serialize as a dictionary
            serializer.Serialize(writer, value);
        }
    }

}
