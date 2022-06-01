namespace TimeSorted;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

// ReSharper disable RedundantCast

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
    /// Source or ID type.
    /// </summary>
    public byte Tag => b0;

    public bool IsEmpty => Equals(Empty);

    /// <summary>
    /// Constructor used for parsing from string or bytes and also creating specific Suid instances such as Empty.
    /// </summary>
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

        if (validate && !IsDateValid(b1, b2, b3, b4, b5, b6))
        {
            ThrowInvalidDateException();
        }
    }

    /// <summary>
    /// Constructor used when generating new Suid from given date and tag and generating random part.
    /// </summary>
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
        var i1 = rand.Next();
        var i2 = rand.Next();
        b7 = (byte)((i1 >> 24) & 0xFF);
        b8 = (byte)((i1 >> 16) & 0xFF);
        b9 = (byte)((i1 >> 8) & 0xFF);
        b10 = (byte)(i1 & 0xFF);
        b11 = (byte)((i2 >> 24) & 0xFF);
        b12 = (byte)((i2 >> 16) & 0xFF);
        b13 = (byte)((i2 >> 8) & 0xFF);
        b14 = (byte)(i2 & 0xFF);
        b15 = (byte)rand.Next(0, 256);
    }

    /// <summary>
    /// Constructor used within for System.Runtime.Serialization.ISerializable deserialization.
    /// </summary>
    private Suid(int i1, int i2, int i3, int i4)
    {
        b0 = (byte)((i1 >> 24) & 0xFF);
        b1 = (byte)((i1 >> 16) & 0xFF);
        b2 = (byte)((i1 >> 8) & 0xFF);
        b3 = (byte)(i1 & 0xFF);

        b4 = (byte)((i2 >> 24) & 0xFF);
        b5 = (byte)((i2 >> 16) & 0xFF);
        b6 = (byte)((i2 >> 8) & 0xFF);
        b7 = (byte)(i2 & 0xFF);

        b8 = (byte)((i3 >> 24) & 0xFF);
        b9 = (byte)((i3 >> 16) & 0xFF);
        b10 = (byte)((i3 >> 8) & 0xFF);
        b11 = (byte)(i3 & 0xFF);

        b12 = (byte)((i4 >> 24) & 0xFF);
        b13 = (byte)((i4 >> 16) & 0xFF);
        b14 = (byte)((i4 >> 8) & 0xFF);
        b15 = (byte)(i4 & 0xFF);

        if (!IsDateValid(b1, b2, b3, b4, b5, b6))
        {
            ThrowInvalidDateException();
        }
    }

    /// <summary>
    /// Constructor used within for System.Runtime.Serialization.ISerializable deserialization.
    /// </summary>
    private Suid(SerializationInfo info, StreamingContext streamingContext)
        : this(info.GetInt32("i1"), info.GetInt32("i2"), info.GetInt32("i3"), info.GetInt32("i4"))
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
        return string.Create(36, this, (chars, s) =>
        {
            const char sep = '-';

            ByteUtil.WriteHex(chars, s.b0, 0);
            ByteUtil.WriteHex(chars, s.b1, 2);
            ByteUtil.WriteHex(chars, s.b2, 4);
            ByteUtil.WriteHex(chars, s.b3, 6);

            chars[8] = sep;

            ByteUtil.WriteHex(chars, s.b4, 9);
            ByteUtil.WriteHex(chars, s.b5, 11);

            chars[13] = sep;

            ByteUtil.WriteHex(chars, s.b6, 14);
            ByteUtil.WriteHex(chars, s.b7, 16);

            chars[18] = sep;

            ByteUtil.WriteHex(chars, s.b8, 19);
            ByteUtil.WriteHex(chars, s.b9, 21);

            chars[23] = sep;

            ByteUtil.WriteHex(chars, s.b10, 24);
            ByteUtil.WriteHex(chars, s.b11, 26);
            ByteUtil.WriteHex(chars, s.b12, 28);
            ByteUtil.WriteHex(chars, s.b13, 30);
            ByteUtil.WriteHex(chars, s.b14, 32);
            ByteUtil.WriteHex(chars, s.b15, 34);
        });
    }

    /// <summary>
    /// Converts the Suid instance to Guid which would have same text representation ToString("D").
    /// </summary>
    public Guid ToGuid()
    {
        int a = (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
        short b = (short)((b4 << 8) | b5);
        short c = (short)((b6 << 8) | b7);
        return new Guid(a, b, c, b8, b9, b10, b11, b12, b13, b14, b15);
    }

    /// <summary>
    /// Returns fixed size byte[16] array, which contains internal Suid bytes in order.
    /// Consider using WriteTo methods rather than creating new byte array.
    /// </summary>
    /// <returns>byte[16]</returns>
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
        => ToUnixTimeMilliseconds(b1, b2, b3, b4, b5, b6);

    public void WriteTo(Span<byte> bytes)
    {
        bytes[0] = b0;
        bytes[1] = b1;
        bytes[2] = b2;
        bytes[3] = b3;
        bytes[4] = b4;
        bytes[5] = b5;
        bytes[6] = b6;
        bytes[7] = b7;
        bytes[8] = b8;
        bytes[9] = b9;
        bytes[10] = b10;
        bytes[11] = b11;
        bytes[12] = b12;
        bytes[13] = b13;
        bytes[14] = b14;
        bytes[15] = b15;
    }

    public void WriteTo(Stream stream)
    {
        stream.WriteByte(b0);
        stream.WriteByte(b1);
        stream.WriteByte(b2);
        stream.WriteByte(b3);
        stream.WriteByte(b4);
        stream.WriteByte(b5);
        stream.WriteByte(b6);
        stream.WriteByte(b7);
        stream.WriteByte(b8);
        stream.WriteByte(b9);
        stream.WriteByte(b10);
        stream.WriteByte(b11);
        stream.WriteByte(b12);
        stream.WriteByte(b13);
        stream.WriteByte(b14);
        stream.WriteByte(b15);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        var i1 = (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
        var i2 = (b4 << 24) | (b5 << 16) | (b6 << 8) | b7;
        var i3 = (b8 << 24) | (b9 << 16) | (b10 << 8) | b11;
        var i4 = (b12 << 24) | (b13 << 16) | (b14 << 8) | b15;

        // ReSharper disable HeapView.BoxingAllocation
        info.AddValue("i1", i1, typeof(int));
        info.AddValue("i2", i2, typeof(int));
        info.AddValue("i3", i3, typeof(int));
        info.AddValue("i4", i4, typeof(int));
    }

    public static bool TryParse(ReadOnlySpan<byte> b, out Suid suid)
    {
        if (b.Length < 16)
        {
            suid = Empty;
            return false;
        }

        suid = new Suid(b[0], b[1], b[2], b[3], b[4], b[5], b[6], b[7], b[8], b[9], b[10], b[11], b[12], b[13], b[14], b[15], true);
        return !suid.IsEmpty;
    }

    public static bool TryParse(ReadOnlySpan<char> s, out Suid suid)
    {
        if (s.Length != 36)
        {
            suid = Empty;
            return false;
        }

        var b0 = ByteUtil.ReadHex(s, 0);
        var b1 = ByteUtil.ReadHex(s, 2);
        var b2 = ByteUtil.ReadHex(s, 4);
        var b3 = ByteUtil.ReadHex(s, 6);

        // separator '-' at index 8
        var b4 = ByteUtil.ReadHex(s, 9);
        var b5 = ByteUtil.ReadHex(s, 11);

        // separator '-' at index 13
        var b6 = ByteUtil.ReadHex(s, 14);
        var b7 = ByteUtil.ReadHex(s, 16);

        // separator '-' at index 18
        var b8 = ByteUtil.ReadHex(s, 19);
        var b9 = ByteUtil.ReadHex(s, 21);

        // separator '-' at index 25
        var b10 = ByteUtil.ReadHex(s, 24);
        var b11 = ByteUtil.ReadHex(s, 26);
        var b12 = ByteUtil.ReadHex(s, 28);
        var b13 = ByteUtil.ReadHex(s, 30);
        var b14 = ByteUtil.ReadHex(s, 32);
        var b15 = ByteUtil.ReadHex(s, 34);

        if (!IsDateValid(b1, b2, b3, b4, b5, b6))
        {
            suid = Empty;
            return false;
        }

        suid = new Suid(b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, false);
        return !suid.IsEmpty;
    }

    public string ToDebugString()
    {
        return $"{ToString()}, {Tag}, {ToDateTimeOffset():yyyy-MM-ddTHH:mm:ss.fff}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ToUnixTimeMilliseconds(byte b1, byte b2,  byte b3,  byte b4, byte b5, byte b6)
    {
        return (long)(((ulong)b1 << 40) | ((ulong)b2 << 32) | ((ulong)b3 << 24) | ((ulong)b4 << 16) | ((ulong)b5 << 8) | (ulong)b6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDateValid(byte b1, byte b2,  byte b3,  byte b4, byte b5, byte b6)
    {
        var milis = ToUnixTimeMilliseconds(b1, b2, b3, b4, b5, b6);
        return milis >= MinValidMs && milis <= MaxValidMs;
    }

    [System.Diagnostics.StackTraceHidden]
    private static void ThrowInvalidDateException()
    {
        throw new InvalidDataException("bytes dedicated to storing date contain date outside of expected range");
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
