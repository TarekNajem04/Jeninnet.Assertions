//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Benchmarks;

/// <summary>Measures reflective object-graph equivalence on a small graph and a large list.</summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class EquivalencyBenchmarks {
    private const int LARGE_GRAPH_SIZE = 500;
    private readonly Person _smallActual = CreatePerson();
    private readonly Person _smallExpected = CreatePerson();
    private readonly Person[] _largeActual = CreatePeople(LARGE_GRAPH_SIZE);
    private readonly Person[] _largeExpected = CreatePeople(LARGE_GRAPH_SIZE);

    /// <summary>Measures equivalence of a three-member object graph.</summary>
    [Benchmark]
    public void BeEquivalentTo_SmallGraph() => _smallActual.Should().BeEquivalentTo(_smallExpected);

    /// <summary>Measures equivalence of a 500-element list of three-member objects.</summary>
    [Benchmark]
    public void BeEquivalentTo_LargeGraph() => _largeActual.Should().BeEquivalentTo(_largeExpected, options => options.AsCollection());

    private static Person CreatePerson() => new() {
        Id = 7,
        Name = "person",
        Address = new Address { City = "city", Zip = "00000" },
    };

    private static Person[] CreatePeople(int count) {
        var people = new Person[count];
        for(var index = 0; index < count; index++) {
            people[index] = CreatePerson();
        }

        return people;
    }

    private sealed class Person {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public Address Address { get; set; } = new();
    }

    private sealed class Address {
        public string City { get; set; } = "";

        public string Zip { get; set; } = "";
    }
}
