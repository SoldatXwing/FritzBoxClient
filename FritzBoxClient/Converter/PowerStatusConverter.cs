using Newtonsoft.Json;

namespace FritzBoxClient.Converter
{
    internal class PowerStatusConverter : JsonConverter<List<string>>
    {
        public override List<string>? ReadJson(JsonReader reader, Type objectType, List<string>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String) // If it's a single string
            {
                return new List<string> { reader.Value?.ToString() ?? string.Empty };
            }
            else if (reader.TokenType == JsonToken.StartArray) // If it's an array of strings
            {
                var statusList = new List<string>();
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        statusList.Add(reader.Value?.ToString() ?? string.Empty);
                    }
                }
                return statusList;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, List<string>? value, JsonSerializer serializer)
        {
            // If value is a list, write it as an array
            if (value != null && value.Count > 0)
            {
                writer.WriteStartArray();
                foreach (var item in value)
                {
                    writer.WriteValue(item); // Write each string item
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteNull(); // If the list is null or empty, write null
            }
        }
    }
}
