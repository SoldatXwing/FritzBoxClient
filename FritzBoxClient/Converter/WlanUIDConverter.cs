using Newtonsoft.Json;

namespace FritzBoxClient.Converter
{
    public class StringToListSeperatedByCommaConverter : JsonConverter<List<string>>
    {
        public override List<string>? ReadJson(JsonReader reader, Type objectType, List<string>? existingValue, bool hasExistingValue, JsonSerializer serializer) => reader.Value?.ToString()!.Split(',').ToList() ?? new List<string>();

        public override void WriteJson(JsonWriter writer, List<string>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}