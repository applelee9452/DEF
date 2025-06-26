using BenchmarkDotNet.Running;

namespace DEF
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<HashFunctionsBenchmark>();

            Console.WriteLine("Hello, World!");
        }
    }
}