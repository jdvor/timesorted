namespace TimeSorted.Tests;

using System.Text.Json.Serialization;

public class ExampleWithUtcDateTime
{
    public UtcDateTime Value1 { get; init; }

    public UtcDateTime? Value2 { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Suid? Value3 { get; init; }
}
