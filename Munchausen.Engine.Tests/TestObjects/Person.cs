namespace Munchausen.Engine.Tests.TestObjects
{
    public class Person
    {
        public Person()
        {
            Age = 0;
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public override string ToString()
        {
            return "Name = " + Name + ", Age = " + Age;
        }
    }
}
