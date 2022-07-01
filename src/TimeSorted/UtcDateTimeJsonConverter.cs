namespace TimeSorted;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class UtcDateTimeJsonConverter : JsonConverter<UtcDateTime>
{
    public override UtcDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && UtcDateTime.TryParse(s, out var utc))
        {
            return utc;
        }

        return UtcDateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, UtcDateTime value, JsonSerializerOptions options)
    {
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        var s = value.ToString();
        writer.WriteStringValue(s);
    }
}
