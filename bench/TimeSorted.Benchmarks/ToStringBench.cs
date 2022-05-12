#pragma warning disable CA1822

namespace TimeSorted.Benchmarks;

using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class ToStringBench
{
    private static readonly Guid G = Guid.NewGuid();
    private static readonly Suid S = Suid.NewSuid(tag: 14);

    [Benchmark(Baseline = true)]
    public string ToStringGuid()
        => G.ToString();

    [Benchmark]
    public string ToStringSuid()
        => S.ToString();
}
