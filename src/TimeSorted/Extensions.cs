namespace TimeSorted;

using System.Text.Json;

public static class Extensions
{
    public static JsonSerializerOptions AppendSuidConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new SuidJsonConverter());
        options.Converters.Add(new NullableSuidJsonConverter());
        return options;
    }

    public static JsonSerializerOptions AppendUtcDateTimeConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new UtcDateTimeJsonConverter());
        options.Converters.Add(new NullableUtcDateTimeJsonConverter());
        return options;
    }
}
