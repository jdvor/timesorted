using BenchmarkDotNet.Running;
using TimeSorted.Benchmarks;

if (args.Length > 0)
{
    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
else
{
    Console.WriteLine("a) NewSuidBench");
    Console.WriteLine("b) ToStringBench");
    Console.WriteLine("c) TryParseBench");
    var key = Console.ReadKey();
    switch (key.Key)
    {
        case ConsoleKey.A:
            BenchmarkRunner.Run<NewSuidBench>();
            break;

        case ConsoleKey.B:
            BenchmarkRunner.Run<ToStringBench>();
            break;

        case ConsoleKey.C:
            BenchmarkRunner.Run<TryParseBench>();
            break;
    }
}
