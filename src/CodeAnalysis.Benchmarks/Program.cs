using BenchmarkDotNet.Running;

namespace Basilisque.CodeAnalysis.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkDotNet.Configs.IConfig? config;
#if DEBUG
            config = new BenchmarkDotNet.Configs.DebugInProcessConfig();
#else
            config = null;
#endif
            BenchmarkRunner.Run(typeof(Program).Assembly, config);
        }
    }
}