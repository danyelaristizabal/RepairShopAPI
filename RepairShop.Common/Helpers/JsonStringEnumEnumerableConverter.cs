using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonStringEnumEnumerableConverter<TEnum> : JsonConverter<IEnumerable<TEnum>> where TEnum : struct
{
    private readonly JsonStringEnumConverter _stringEnumConverter = new();

    public override IEnumerable<TEnum> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var enumValues = new List<TEnum>();

        foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
        {
            var enumString = jsonElement.GetString();
            if (enumString != null && Enum.TryParse(enumString, out TEnum enumValue))
            {
                enumValues.Add(enumValue);
            }
        }

        return enumValues.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<TEnum> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var enumValue in value)
        {
            writer.WriteStringValue(enumValue.ToString());
        }

        writer.WriteEndArray();
    }
}
