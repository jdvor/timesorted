namespace TimeSorted;

using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
public readonly struct UtcDateTime
    : IComparable, ISpanFormattable, IConvertible, IComparable<UtcDateTime>, IEquatable<UtcDateTime>, ISerializable
{
    public const string DefaultStringFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public const int MinTicks = 0;

    // DateTime.cs: internal const long MaxTicks = DaysTo10000 * TicksPerDay - 1;
    public const long MaxTicks = (3652059 * 864_000_000_000) - 1;

    // DateTime.cs: internal const long UnixEpochTicks = DaysTo1970 * TicksPerDay;
    public const long UnixEpochTicks = 719_162 * 864_000_000_000;

    private const string SerializationInfoKey = "ticks";

    public static readonly UtcDateTime MinValue = new UtcDateTime(DateTime.MinValue);

    public static readonly UtcDateTime MaxValue = new UtcDateTime(DateTime.MaxValue);

    [FieldOffset(0)]
    private readonly DateTime value;

    public DateTime Value => value;

    public UtcDateTime(DateTime dt)
    {
        value = dt.ToUniversalTime();
    }

    #region Equality

    public bool Equals(UtcDateTime other)
        => value.Equals(value);

    public override bool Equals(object? obj)
        => obj is UtcDateTime other && Equals(other);

    public override int GetHashCode()
        => value.GetHashCode();

    public static bool operator ==(UtcDateTime left, UtcDateTime right)
        => left.Equals(right);

    public static bool operator !=(UtcDateTime left, UtcDateTime right)
        => !left.Equals(right);

    #endregion

    #region Comparison

    public int CompareTo(UtcDateTime other)
        => value.CompareTo(other);

    public int CompareTo(object? obj)
        => obj is UtcDateTime other ? CompareTo(other) : throw new InvalidOperationException();

    public static bool operator <(UtcDateTime left, UtcDateTime right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(UtcDateTime left, UtcDateTime right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(UtcDateTime left, UtcDateTime right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(UtcDateTime left, UtcDateTime right)
        => left.CompareTo(right) >= 0;

    #endregion

    #region Conversion

    public DateTimeOffset ToDateTimeOffset()
        => new DateTimeOffset(value, TimeSpan.Zero);

    public static UtcDateTime From(DateTimeOffset dto)
        => new UtcDateTime(dto.ToUniversalTime().DateTime);

    public DateOnly ToDateOnly()
        => new DateOnly(value.Year, value.Month, value.Day);

    public static UtcDateTime From(DateOnly d)
        => new UtcDateTime(d.ToDateTime(new TimeOnly(0), DateTimeKind.Utc));

    public static UtcDateTime FromUnixTimeMilliseconds(long milliseconds)
    {
        var ticks = (milliseconds * TimeSpan.TicksPerMillisecond) + UnixEpochTicks;
        return new UtcDateTime(new DateTime(ticks, DateTimeKind.Utc));
    }

    public static UtcDateTime FromUnixTimeSeconds(long seconds)
    {
        var ticks = (seconds * TimeSpan.TicksPerSecond) + UnixEpochTicks;
        return new UtcDateTime(new DateTime(ticks, DateTimeKind.Utc));
    }

    public static implicit operator DateTime(UtcDateTime utc)
        => utc.value;

    public static explicit operator DateTimeOffset(UtcDateTime utc)
        => utc.ToDateTimeOffset();

    public static explicit operator DateOnly(UtcDateTime utc)
        => utc.ToDateOnly();

    public TypeCode GetTypeCode()
        => value.GetTypeCode();

    public bool ToBoolean(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Boolean));

    public byte ToByte(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Byte));

    public char ToChar(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Char));

    public DateTime ToDateTime(IFormatProvider? provider)
        => value;

    public decimal ToDecimal(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Decimal));

    public double ToDouble(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Double));

    public short ToInt16(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Int16));

    public int ToInt32(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Int32));

    public long ToInt64(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Int64));

    public sbyte ToSByte(IFormatProvider? provider)
        => throw InvalidConversion(nameof(SByte));

    public float ToSingle(IFormatProvider? provider)
        => throw InvalidConversion(nameof(Single));

    public string ToString(IFormatProvider? provider)
        => value.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        if (conversionType == null)
        {
            throw new ArgumentNullException(nameof(conversionType));
        }

        if (ReferenceEquals(GetType(), conversionType))
        {
            return this;
        }

        var typeCode = Type.GetTypeCode(conversionType);
        return typeCode switch
        {
            TypeCode.Boolean => ToBoolean(provider),
            TypeCode.Char => ToChar(provider),
            TypeCode.SByte => ToSByte(provider),
            TypeCode.Byte => ToByte(provider),
            TypeCode.Int16 => ToInt16(provider),
            TypeCode.UInt16 => ToUInt16(provider),
            TypeCode.Int32 => ToInt32(provider),
            TypeCode.UInt32 => ToUInt32(provider),
            TypeCode.Int64 => ToInt64(provider),
            TypeCode.UInt64 => ToUInt64(provider),
            TypeCode.Single => ToSingle(provider),
            TypeCode.Double => ToDouble(provider),
            TypeCode.Decimal => ToDecimal(provider),
            TypeCode.DateTime => value,
            TypeCode.String => ToString(provider),
            TypeCode.Object => this,
            TypeCode.DBNull => throw InvalidConversion(conversionType.Name),
            TypeCode.Empty => throw InvalidConversion(conversionType.Name),
            _ => throw InvalidConversion(conversionType.Name),
        };
    }

    public ushort ToUInt16(IFormatProvider? provider)
        => throw InvalidConversion(nameof(UInt16));

    public uint ToUInt32(IFormatProvider? provider)
        => throw InvalidConversion(nameof(UInt32));

    public ulong ToUInt64(IFormatProvider? provider)
        => throw InvalidConversion(nameof(UInt64));

    private static Exception InvalidConversion(string to)
        => new InvalidCastException($"conversion from {to} to {typeof(UtcDateTime).Name} is not supported");

    #endregion

    #region Format

    public string ToString(string? format, IFormatProvider? formatProvider)
        => value.ToString(formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => value.TryFormat(destination, out charsWritten, format, provider);

    public override string ToString()
        => value.ToString(DefaultStringFormat, CultureInfo.InvariantCulture);

    #endregion

    #region Serialization

    private UtcDateTime(SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
        var v = serializationInfo.GetValue(SerializationInfoKey, typeof(long));
        if (v is long ticks && ticks >= MinTicks && ticks <= MaxTicks)
        {
            value = new DateTime(ticks, DateTimeKind.Utc);
        }

        value = DateTime.MinValue;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(SerializationInfoKey, value.Ticks);
    }

    public static bool TryParse(ReadOnlySpan<char> s, out UtcDateTime utc)
    {
        if (DateTime.TryParse(s, out var dt))
        {
            utc = new UtcDateTime(dt);
            return true;
        }

        utc = MinValue;
        return false;
    }

    public static bool TryParse(string s, out UtcDateTime utc)
    {
        if (DateTime.TryParse(s, out var dt))
        {
            utc = new UtcDateTime(dt);
            return true;
        }

        utc = MinValue;
        return false;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, DateTimeStyles styles, out UtcDateTime utc)
    {
        if (DateTime.TryParse(s, provider, styles, out var dt))
        {
            utc = new UtcDateTime(dt);
            return true;
        }

        utc = MinValue;
        return false;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        DateTimeStyles styles,
        out UtcDateTime utc)
    {
        if (DateTime.TryParse(s, provider, styles, out var dt))
        {
            utc = new UtcDateTime(dt);
            return true;
        }

        utc = MinValue;
        return false;
    }

    public static UtcDateTime Parse(
        string? s,
        IFormatProvider? provider = null,
        DateTimeStyles styles = DateTimeStyles.None)
    {
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var dt = DateTime.Parse(s, provider, styles);
        return new UtcDateTime(dt);
    }

    public static UtcDateTime Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = null,
        DateTimeStyles styles = DateTimeStyles.None)
    {
        var dt = DateTime.Parse(s, provider, styles);
        return new UtcDateTime(dt);
    }

    #endregion

    #region Other

    public static UtcDateTime Now()
        => new UtcDateTime(DateTime.UtcNow);

    public static TimeSpan operator -(UtcDateTime left, UtcDateTime right)
        => left.value - right.value;

    public static UtcDateTime operator -(UtcDateTime left, TimeSpan right)
        => new UtcDateTime(left.value - right);

    public static UtcDateTime operator +(UtcDateTime left, TimeSpan right)
        => new UtcDateTime(left.value + right);

    #endregion
}
