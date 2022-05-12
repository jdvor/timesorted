namespace TimeSorted.Tests;

using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void Comparable()
    {
        var asc = AscendingExamples();
        for (var i = 0; i < asc.Length - 1; i += 2)
        {
            var left = asc[i];
            var right = asc[i + 1];
            var cmp = left.CompareTo(right);
            Assert.True(cmp == -1, $"failed {nameof(AscendingExamples)}[{i}] < {nameof(AscendingExamples)}[{i + 1}]");
        }
    }

    [Fact]
    public void StringRepresentationIsSameAsGuid()
    {
        var s1 = Suid.NewSuid(tag: 17);
        var actual = s1.ToString();
        var g1 = s1.ToGuid();
        var expected = g1.ToString("D");

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2022, 5, 11, 14, 28, 51, 678)]
    public void DatePartSerialization(int year, int month, int day, int hour, int minute, int sec, int ms)
    {
        var expectedDate = new DateTimeOffset(year, month, day, hour, minute, sec, ms, TimeSpan.Zero);
        var expectedMs = expectedDate.ToUnixTimeMilliseconds();

        var s1 = Suid.NewSuid(expectedDate, tag: 17);
        var actualDate = s1.ToDateTimeOffset();
        var actualMs = s1.ToUnixTimeMilliseconds();

        Assert.Equal(expectedDate, actualDate);
        Assert.Equal(expectedMs, actualMs);
    }

    [Fact]
    public void StringSerialization()
    {
        var expected = Suid.NewSuid(tag: 8);
        var str1 = expected.ToString();
        var ok = Suid.TryParse(str1, out var actual);

        Assert.True(ok);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SystemRuntimeSerializable()
    {
        #pragma warning disable SYSLIB0011

        var expected = Suid.NewSuid(tag: 17);

        var bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf.Serialize(ms, expected);
        ms.Position = 0;
        var deserialized = (Suid)bf.Deserialize(ms);

        Assert.Equal(expected, deserialized);

        #pragma warning restore SYSLIB0011
    }

    public static Suid[] AscendingExamples()
    {
        return new[]
        {
            Suid.NewSuid(new DateTimeOffset(2021, 1, 1, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 1, 1, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 4, 1, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 1, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 10, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 0, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 13, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 0, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 27, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 28, 0, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 28, 50, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 28, 51, 0, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 28, 51, 678, TimeSpan.Zero), tag: 17),
            Suid.NewSuid(new DateTimeOffset(2022, 5, 11, 14, 28, 51, 679, TimeSpan.Zero), tag: 17),
        };
    }
}
