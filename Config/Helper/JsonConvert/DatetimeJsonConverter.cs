using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Config
{
    public class DatetimeJsonConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (string.IsNullOrEmpty(reader.GetString()))
                    return null;
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteStringValue("");
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
   
}
