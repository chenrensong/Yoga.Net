using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Yoga.Net.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "--simple")
        {
            // Run simple benchmark for quick results
            SimpleBenchmark.Run();
        }
        else
        {
            // Run BenchmarkDotNet benchmarks
            var config = DefaultConfig.Instance
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}
