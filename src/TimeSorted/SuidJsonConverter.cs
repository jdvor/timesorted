namespace TimeSorted;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class SuidJsonConverter : JsonConverter<Suid>
{
    public override Suid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s is not null && Suid.TryParse(s, out var suid))
        {
            return suid;
        }

        return Suid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Suid value, JsonSerializerOptions options)
    {
        var s = value.ToString();
        writer.WriteStringValue(s);
    }
}
