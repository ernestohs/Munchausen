using BenchmarkDotNet.Attributes;
using Munchausen;
using Munchausen.Runtime;
using Munchausen.TestModels;

namespace Munchausen.Benchmarks;

/// <summary>
/// The v1.0 performance budgets from ARCHITECTURE.md: cold/warm Build, steady-state
/// per-object generation, collection materialization, and PRNG throughput. Tracked
/// for trend in CI, not asserted here.
/// </summary>
[MemoryDiagnoser]
public class GenerationBenchmarks
{
    private static readonly GenerationOptions Seeded = new() { Seed = 42 };
    private LieDefinition<Car> _carDefinition = null!;
    private LieDefinition<Customer> _customerDefinition = null!;
    private DeterministicRandom _random = null!;

    [GlobalSetup]
    public void Setup()
    {
        _carDefinition = Lie.Define<Car>().Build();
        _customerDefinition = Lie.Define<Customer>().Build();
        _random = new DeterministicRandom(42);
    }

    [Benchmark]
    public LieDefinition<Car> BuildWarm() => Lie.Define<Car>().Build();

    [Benchmark]
    public Car GenerateCar() => _carDefinition.Generate(Seeded);

    [Benchmark]
    public IReadOnlyList<Car> GenerateCars() => _carDefinition.Generate(1000, Seeded);

    [Benchmark]
    public Customer GenerateCustomerWithCollection() => _customerDefinition.Generate(Seeded);

    [Benchmark]
    public ulong PrngThroughput()
    {
        ulong accumulator = 0;
        for (int i = 0; i < 1000; i++)
        {
            accumulator ^= _random.NextULong();
        }

        return accumulator;
    }
}
