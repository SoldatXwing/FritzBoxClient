using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FritzBoxClient.Converter
{
    public class BoolConverter : Newtonsoft.Json.JsonConverter<bool>
    {
        public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer) => reader.Value?.ToString() == "1";


        public override void WriteJson(JsonWriter writer, bool value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
