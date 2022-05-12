namespace TimeSorted;

using System.Runtime.CompilerServices;

internal static class ByteUtil
{
    private static readonly uint[] Lookup32 = CreateLookup32();

    private static uint[] CreateLookup32()
    {
        var result = new uint[256];
        for (var i = 0; i < 256; i++)
        {
            var s = i.ToString("X2").ToLowerInvariant();
            result[i] = s[0] + ((uint)s[1] << 16);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteHex(Span<char> chars, byte value, int position = 0)
    {
        var b = Lookup32[value];
        chars[position] = (char)b;
        chars[position + 1] = (char)(b >> 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte ReadHex(ReadOnlySpan<char> chars, int position = 0)
    {
        var hi = (int)chars[position];
        var lo = (int)chars[position + 1];

        hi = hi switch
        {
            >= 48 and <= 57 => hi - 48, // char: 0..9
            >= 65 and <= 70 => hi - 55, // char: A..F
            >= 97 and <= 102 => hi - 87, // char: a..f
            _ => 0,
        };

        lo = lo switch
        {
            >= 48 and <= 57 => lo - 48,
            >= 65 and <= 70 => lo - 55,
            >= 97 and <= 102 => lo - 87,
            _ => 0,
        };

        return (byte)((hi << 4) | lo);
    }
}
