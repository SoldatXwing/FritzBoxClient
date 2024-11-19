using FritzBoxClient.Models.ErrorModels;
using Newtonsoft.Json;

namespace FritzBoxClient.Converter
{
    public class ErrorDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<ErrorData>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var message = serializer.Deserialize<string>(reader);
                return new List<ErrorData>
            {
                new() { Message = message!, Path = null!, Code = 0 }
            };
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer!.Deserialize<List<ErrorData>>(reader)!;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var singleErrorData = serializer.Deserialize<ErrorData>(reader);
                return new List<ErrorData> { singleErrorData! };
            }

            return new List<ErrorData>();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}
