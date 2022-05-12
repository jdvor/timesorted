#pragma warning disable CA1822

namespace TimeSorted.Benchmarks;

using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class TryParseBench
{
    private static readonly string GuidStr = Guid.NewGuid().ToString();
    private static readonly string SuidStr = Suid.NewSuid(tag: 14).ToString();

    [Benchmark(Baseline = true)]
    public bool TryParseGuid()
        => Guid.TryParse(GuidStr, out var g);

    [Benchmark]
    public bool TryParseSuid()
        => Suid.TryParse(SuidStr, out var s);
}
