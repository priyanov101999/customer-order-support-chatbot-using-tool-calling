using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace CustomerSupport.Api.Converters;
public class NullableInt32Converter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (int.TryParse(reader.GetString(), out var value))
                return value;
            return null;
        }
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt32();
        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}