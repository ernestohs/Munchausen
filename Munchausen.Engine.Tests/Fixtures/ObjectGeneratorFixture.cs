using System;
using Munchausen.Engine.Tests.TestObjects;
using NUnit.Framework;

namespace Munchausen.Engine.Tests.Fixtures
{
    [TestFixture]
    public class ObjectGeneratorFixture
    {
        [Test]
        public void GeneratorCanonical()
        {
            Object newInstance = new ObjectGenerator<Person>().Generate();

            Assert.That(newInstance, Is.Not.Null);
            Assert.That(newInstance, Is.TypeOf<Person>());
        }
    }
}
