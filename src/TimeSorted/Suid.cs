namespace TimeSorted;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

/// <summary>
/// Time-sortable GUID-like identifier with one byte dedicated to tagging the source or type of ID.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Explicit)]
[DebuggerDisplay("{ToDebugString()}")]
public readonly struct Suid : IEquatable<Suid>, IComparable<Suid>, IComparable, ISerializable
{
    /// <summary>
    /// Minimum epoch milliseconds; represents date 2000-01-01T00:00:00.000Z.
    /// </summary>
    private const long MinValidMs = 946_681_200_000;

    /// <summary>
    /// Maximum epoch milliseconds; represents date 10889-08-02T05:31:50.656Z.
    /// </summary>
    private const long MaxValidMs = 281_474_976_710_656; // 2^48, 6 bytes

    private static readonly ThreadLocal<Random> Rand = new(() => new Random(Interlocked.Increment(ref seed)));
    private static int seed = Environment.TickCount;

    /// <summary>
    /// Represents default value and also usually invalid or not-known instance.
    /// </summary>
    public static readonly Suid Empty = new Suid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);

    #region data fields (explicit layout)

    [FieldOffset(0)]
    private readonly byte b0;

    [FieldOffset(1)]
    private readonly byte b1;

    [FieldOffset(2)]
    private readonly byte b2;

    [FieldOffset(3)]
    private readonly byte b3;

    [FieldOffset(4)]
    private readonly byte b4;

    [FieldOffset(5)]
    private readonly byte b5;

    [FieldOffset(6)]
    private readonly byte b6;

    [FieldOffset(7)]
    private readonly byte b7;

    [FieldOffset(8)]
    private readonly byte b8;

    [FieldOffset(9)]
    private readonly byte b9;

    [FieldOffset(10)]
    private readonly byte b10;

    [FieldOffset(11)]
    private readonly byte b11;

    [FieldOffset(12)]
    private readonly byte b12;

    [FieldOffset(13)]
    private readonly byte b13;

    [FieldOffset(14)]
    private readonly byte b14;

    [FieldOffset(15)]
    private readonly byte b15;

    #endregion

    /// <summary>
    /// ???
    /// </summary>
    public byte Tag => b0;

    public bool IsEmpty => Equals(Empty);

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:Parameters should be on same line or separate lines",
        Justification = "separate lines occupies too much of vertical space")]
    private Suid(byte b0, byte b1, byte b2,  byte b3,  byte b4, byte b5, byte b6,  byte b7,  byte b8, byte b9,
        byte b10,  byte b11,  byte b12, byte b13, byte b14,  byte b15, bool validate)
    {
        this.b0 = b0;
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
        this.b4 = b4;
        this.b5 = b5;
        this.b6 = b6;
        this.b7 = b7;
        this.b8 = b8;
        this.b9 = b9;
        this.b10 = b10;
        this.b11 = b11;
        this.b12 = b12;
        this.b13 = b13;
        this.b14 = b14;
        this.b15 = b15;

        if (validate)
        {
            ValidateInternalData();
        }
    }

    private Suid(DateTimeOffset date, byte tag)
    {
        b0 = tag;

        // big-endian (most significant bytes first)
        var millis = date.ToUnixTimeMilliseconds();
        b1 = (byte)((millis >> 40) & 0xFF);
        b2 = (byte)((millis >> 32) & 0xFF);
        b3 = (byte)((millis >> 24) & 0xFF);
        b4 = (byte)((millis >> 16) & 0xFF);
        b5 = (byte)((millis >> 8) & 0xFF);
        b6 = (byte)(millis & 0xFF);

        // randomly generated portion of the Suid
        var rand = Rand.Value!;
        b7 = (byte)rand.Next(0, 256);
        b8 = (byte)rand.Next(0, 256);
        b9 = (byte)rand.Next(0, 256);
        b10 = (byte)rand.Next(0, 256);
        b11 = (byte)rand.Next(0, 256);
        b12 = (byte)rand.Next(0, 256);
        b13 = (byte)rand.Next(0, 256);
        b14 = (byte)rand.Next(0, 256);
        b15 = (byte)rand.Next(0, 256);
    }

    private Suid(long hi, long lo)
    {
        b0 = (byte)((hi >> 56) & 0xFF);
        b1 = (byte)((hi >> 48) & 0xFF);
        b2 = (byte)((hi >> 40) & 0xFF);
        b3 = (byte)((hi >> 32) & 0xFF);
        b4 = (byte)((hi >> 24) & 0xFF);
        b5 = (byte)((hi >> 16) & 0xFF);
        b6 = (byte)((hi >> 8) & 0xFF);
        b7 = (byte)(hi & 0xFF);

        b8 = (byte)((lo >> 56) & 0xFF);
        b9 = (byte)((lo >> 48) & 0xFF);
        b10 = (byte)((lo >> 40) & 0xFF);
        b11 = (byte)((lo >> 32) & 0xFF);
        b12 = (byte)((lo >> 24) & 0xFF);
        b13 = (byte)((lo >> 16) & 0xFF);
        b14 = (byte)((lo >> 8) & 0xFF);
        b15 = (byte)(lo & 0xFF);

        ValidateInternalData();
    }

    private Suid(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : this(serializationInfo.GetInt64("High"), serializationInfo.GetInt64("Low"))
    {
    }

    public static Suid NewSuid(DateTimeOffset date, byte tag = 0)
        => new Suid(date, tag);

    public static Suid NewSuid(byte tag = 0)
        => new Suid(DateTimeOffset.UtcNow, tag);

    public bool Equals(Suid other)
    {
        return b0.Equals(other.b0)
               && b1.Equals(other.b1)
               && b2.Equals(other.b2)
               && b3.Equals(other.b3)
               && b4.Equals(other.b4)
               && b5.Equals(other.b5)
               && b6.Equals(other.b6)
               && b7.Equals(other.b7)
               && b8.Equals(other.b8)
               && b9.Equals(other.b9)
               && b10.Equals(other.b10)
               && b11.Equals(other.b11)
               && b12.Equals(other.b12)
               && b13.Equals(other.b13)
               && b14.Equals(other.b14)
               && b15.Equals(other.b15);
    }

    public override bool Equals(object? obj)
        => obj is Suid suid && Equals(suid);

    public override int GetHashCode()
    {
        unchecked
        {
            int h = 27;
            h = (13 * h) + b0.GetHashCode();
            h = (13 * h) + b1.GetHashCode();
            h = (13 * h) + b2.GetHashCode();
            h = (13 * h) + b3.GetHashCode();
            h = (13 * h) + b4.GetHashCode();
            h = (13 * h) + b5.GetHashCode();
            h = (13 * h) + b6.GetHashCode();
            h = (13 * h) + b7.GetHashCode();
            h = (13 * h) + b8.GetHashCode();
            h = (13 * h) + b9.GetHashCode();
            h = (13 * h) + b10.GetHashCode();
            h = (13 * h) + b11.GetHashCode();
            h = (13 * h) + b12.GetHashCode();
            h = (13 * h) + b13.GetHashCode();
            h = (13 * h) + b14.GetHashCode();
            h = (13 * h) + b15.GetHashCode();
            return h;
        }
    }

    public int CompareTo(Suid other)
    {
        #pragma warning disable SA1503, SA1107
        if (b0 > other.b0) return 1; if (b0 < other.b0) return -1;
        if (b1 > other.b1) return 1; if (b1 < other.b1) return -1;
        if (b2 > other.b2) return 1; if (b2 < other.b2) return -1;
        if (b3 > other.b3) return 1; if (b3 < other.b3) return -1;
        if (b4 > other.b4) return 1; if (b4 < other.b4) return -1;
        if (b5 > other.b5) return 1; if (b5 < other.b5) return -1;
        if (b6 > other.b6) return 1; if (b6 < other.b6) return -1;
        if (b7 > other.b7) return 1; if (b7 < other.b7) return -1;
        if (b8 > other.b8) return 1; if (b8 < other.b8) return -1;
        if (b9 > other.b9) return 1; if (b9 < other.b9) return -1;
        if (b10 > other.b10) return 1; if (b10 < other.b10) return -1;
        if (b11 > other.b11) return 1; if (b11 < other.b11) return -1;
        if (b12 > other.b12) return 1; if (b12 < other.b12) return -1;
        if (b13 > other.b13) return 1; if (b13 < other.b13) return -1;
        if (b14 > other.b14) return 1; if (b14 < other.b14) return -1;
        if (b15 > other.b15) return 1; if (b15 < other.b15) return -1;
        return 0;
        #pragma warning restore SA1503, SA1107
    }

    public int CompareTo(object? obj)
        => obj is Suid suid ? CompareTo(suid) : -1;

    public override string ToString()
    {
        const char sep = '-';
        var chars = new char[36];

        ByteUtil.WriteHex(chars, b0, 0);
        ByteUtil.WriteHex(chars, b1, 2);
        ByteUtil.WriteHex(chars, b2, 4);
        ByteUtil.WriteHex(chars, b3, 6);

        chars[8] = sep;

        ByteUtil.WriteHex(chars, b4, 9);
        ByteUtil.WriteHex(chars, b5, 11);

        chars[13] = sep;

        ByteUtil.WriteHex(chars, b6, 14);
        ByteUtil.WriteHex(chars, b7, 16);

        chars[18] = sep;

        ByteUtil.WriteHex(chars, b8, 19);
        ByteUtil.WriteHex(chars, b9, 21);

        chars[23] = sep;

        ByteUtil.WriteHex(chars, b10, 24);
        ByteUtil.WriteHex(chars, b11, 26);
        ByteUtil.WriteHex(chars, b12, 28);
        ByteUtil.WriteHex(chars, b13, 30);
        ByteUtil.WriteHex(chars, b14, 32);
        ByteUtil.WriteHex(chars, b15, 34);

        return new string(chars);
    }

    public Guid ToGuid()
    {
        throw new NotImplementedException();
    }

    public byte[] ToByteArray()
    {
        return new[] { b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15 };
    }

    public DateTime ToUtcDateTime()
    {
        return ToDateTimeOffset().UtcDateTime;
    }

    public DateTimeOffset ToDateTimeOffset()
    {
        var ms = ToUnixTimeMilliseconds();
        return DateTimeOffset.FromUnixTimeMilliseconds(ms);
    }

    public long ToUnixTimeMilliseconds()
    {
        return ((long)b1 << 40) + ((long)b2 << 32) + ((long)b3 << 24) + (b4 << 16) + (b5 << 8) + b6;
    }

    private void ValidateInternalData()
    {
        var milis = ToUnixTimeMilliseconds();
        if (milis > MaxValidMs || milis < MinValidMs)
        {
            throw new InvalidDataException("bytes dedicated to storing date contain date outside of expected range");
        }
    }

    public (long High, long Low) ToHighLowInt64()
    {
        var high = ((long)b0 << 48) + ((long)b1 << 40) + ((long)b3 << 32) + (b4 << 24) + (b5 << 16) + (b6 << 8) + b7;
        var low = ((long)b8 << 48) + ((long)b9 << 40) + ((long)b10 << 32) + (b11 << 24) + (b12 << 16) + (b13 << 8) + b14;
        return (high, low);
    }

    public string ToDebugString()
    {
        return $"{ToString()}, {Tag}, {ToDateTimeOffset():s}";
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        var (high, low) = ToHighLowInt64();
        info.AddValue("High", high, typeof(long));
        info.AddValue("Low", low, typeof(long));
    }

    public static bool TryParse(ReadOnlySpan<byte> b, out Suid suid)
    {
        if (b.Length != 16)
        {
            suid = Empty;
            return false;
        }

        suid = new Suid(b[0], b[1], b[2], b[3], b[4], b[5], b[6], b[7], b[8], b[9], b[10], b[11], b[12], b[13], b[14], b[15], true);
        return true;
    }

    #region Operator overloads

    public static bool operator ==(Suid left, Suid right)
        => left.Equals(right);

    public static bool operator !=(Suid left, Suid right)
        => !left.Equals(right);

    public static bool operator <(Suid left, Suid right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(Suid left, Suid right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(Suid left, Suid right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(Suid left, Suid right)
        => left.CompareTo(right) >= 0;

    #endregion
}
