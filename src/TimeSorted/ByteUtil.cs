namespace TimeSorted;

using System.Runtime.CompilerServices;

internal static class ByteUtil
{
    private static readonly uint[] Lookup32 = CreateLookup32();

    private static readonly Dictionary<char, byte> HexLookup = new Dictionary<char, byte>()
    {
        { 'a', 0xA }, { 'b', 0xB }, { 'c', 0xC }, { 'd', 0xD },
        { 'e', 0xE }, { 'f', 0xF }, { 'A', 0xA }, { 'B', 0xB },
        { 'C', 0xC }, { 'D', 0xD }, { 'E', 0xE }, { 'F', 0xF },
        { '0', 0x0 }, { '1', 0x1 }, { '2', 0x2 }, { '3', 0x3 },
        { '4', 0x4 }, { '5', 0x5 }, { '6', 0x6 }, { '7', 0x7 },
        { '8', 0x8 }, { '9', 0x9 },
    };

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
        var ch1 = chars[position];
        var ch2 = chars[position + 1];
        return (byte)((HexLookup[ch1] << 4) | HexLookup[ch2]);
    }
}
