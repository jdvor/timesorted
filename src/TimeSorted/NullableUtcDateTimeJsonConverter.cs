namespace TimeSorted;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class NullableUtcDateTimeJsonConverter : JsonConverter<UtcDateTime?>
{
    public override UtcDateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && UtcDateTime.TryParse(s, out var utc))
        {
            return utc;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, UtcDateTime? value, JsonSerializerOptions options)
    {
        if (value is not null)
        {
            var s = value.ToString();
            writer.WriteStringValue(s);
        }

        if (options.DefaultIgnoreCondition != JsonIgnoreCondition.Never)
        {
            writer.WriteNullValue();
        }
    }
}
