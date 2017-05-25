using System.Linq;
using FluentAssertions;
using Munchausen.Tests.TestObjects;
using Xunit;

namespace Munchausen.Tests
{
    public class LieGeneratorTests
    {
        [Fact]
        public void GenerateSingleObjectTest()
        {
            var person = Lie<Person>.Generate();

            person.Should().NotBeNull();
            person.Should().BeOfType<Person>();
        }

        [Fact]
        public void GenerateTenObjectsTest()
        {
            var persons = Lie<Person>.Generate(10);

            persons.Should().NotBeNull();
            persons.Should().NotContainNulls();
            persons.Count.Should().Be(10);
            persons.Should().AllBeOfType<Person>();
        }

        [Fact]
        public void GenerateTenObjectsWithSequenceTest()
        {
            var persons = Lie<Person>.Generate(10);

            persons.Should().NotBeNull();
            persons.Should().NotContainNulls();

            persons.Count
                .Should().Be(10);

            persons.Select(x => x.Age).Distinct().Count()
                .Should().Be(persons.Count);

            persons.Should().BeInAscendingOrder(p => p.Age);
        }

    }
}
