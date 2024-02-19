using System.Text.Json;
using System.Text.Json.Serialization;

public class NullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        if (Guid.TryParse(str, out var guid))
        {
            return guid;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}
