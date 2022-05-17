using BenchmarkDotNet.Running;
using TimeSorted.Benchmarks;

if (args.Length > 0)
{
    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
else
{
    BenchmarkRunner.Run<ToStringBench>();
}
