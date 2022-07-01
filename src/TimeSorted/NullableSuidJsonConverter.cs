namespace TimeSorted;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class NullableSuidJsonConverter : JsonConverter<Suid?>
{
    public override Suid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && Suid.TryParse(s, out var suid))
        {
            return suid;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Suid? value, JsonSerializerOptions options)
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
