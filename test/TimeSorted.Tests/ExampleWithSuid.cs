namespace TimeSorted.Tests;

using System.Text.Json.Serialization;

public class ExampleWithSuid
{
    public Suid Value1 { get; init; }

    public Suid? Value2 { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Suid? Value3 { get; init; }
}
