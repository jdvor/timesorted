#pragma warning disable CA1822

namespace TimeSorted.Benchmarks;

using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class NewSuidBench
{
    [Benchmark(Baseline = true)]
    public Guid NewGuid()
        => Guid.NewGuid();

    [Benchmark]
    public Suid NewSuid()
        => Suid.NewSuid(tag: 14);
}
