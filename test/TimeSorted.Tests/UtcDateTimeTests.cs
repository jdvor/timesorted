namespace TimeSorted.Tests;

using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Xunit;

public class UtcDateTimeTests
{
    private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions().AppendUtcDateTimeConverters();

    [Fact]
    public void JsonSerializable()
    {
        var expected = UtcDateTime.Now();

        var json = JsonSerializer.Serialize(expected, JsonOpts);
        var deserialized = JsonSerializer.Deserialize<UtcDateTime>(json, JsonOpts);

        Assert.Equal(expected, deserialized);
    }

    [Fact]
    public void JsonSerializableWithNull()
    {
        var expected = new ExampleWithUtcDateTime
        {
            Value1 = UtcDateTime.Now(),
        };

        var json = JsonSerializer.Serialize(expected, JsonOpts);
        var deserialized = JsonSerializer.Deserialize<ExampleWithUtcDateTime>(json, JsonOpts)!;

        Assert.Equal(expected.Value1, deserialized.Value1);
        Assert.Equal(expected.Value2, deserialized.Value2);
        Assert.Equal(expected.Value3, deserialized.Value3);
        Assert.DoesNotContain("Value3", json);
    }

    [Fact]
    public void SystemRuntimeSerializable()
    {
        #pragma warning disable SYSLIB0011

        var expected = UtcDateTime.Now();

        var bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf.Serialize(ms, expected);
        ms.Position = 0;
        var deserialized = (UtcDateTime)bf.Deserialize(ms);

        Assert.Equal(expected, deserialized);

        #pragma warning restore SYSLIB0011
    }

    public static UtcDateTime[] AscendingExamples()
    {
        return new[]
        {
            UtcDateTime.Parse("2022-01-01T00:00:00.000Z"),
            UtcDateTime.Parse("2022-06-13T20:35:14.950Z"),
            UtcDateTime.Parse("2022-06-13T20:35:14.951Z"),
            UtcDateTime.Parse("2022-06-17T14:43:26.000Z"),
        };
    }
}
