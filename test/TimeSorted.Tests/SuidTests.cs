namespace TimeSorted.Tests;

using Xunit;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var s1 = Suid.NewSuid(tag: 17);
        var str1 = s1.ToString();
        var bts = s1.ToByteArray();
        var btsDbg = ByteUtil.DebugBytes(bts);
    }
}
