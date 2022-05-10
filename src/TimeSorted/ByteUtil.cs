namespace TimeSorted;

using System.Runtime.CompilerServices;
using System.Text;

internal static class ByteUtil
{
    private static readonly uint[] Lookup32 = CreateLookup32();

    private static uint[] CreateLookup32()
    {
        var result = new uint[256];
        for (var i = 0; i < 256; i++)
        {
            var s = i.ToString("X2");
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

    internal static string DebugBytes(ReadOnlySpan<byte> bytes)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < bytes.Length; i++)
        {
            var b = bytes[i];
            sb.AppendFormat("{0}: byte {1}, hex {1:X2}\r\n", i, b);
            if (i == 3 || i == 5 || i == 7 || i == 9)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
