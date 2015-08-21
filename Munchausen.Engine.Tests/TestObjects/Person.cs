namespace Munchausen.Engine.Tests.TestObjects
{
    public class Person
    {
        private string myName = "N/A";
        private int myAge = 0;

        public string Name
        {
            get
            {
                return myName;
            }
            set
            {
                myName = value;
            }
        }

        public int Age
        {
            get
            {
                return myAge;
            }
            set
            {
                myAge = value;
            }
        }

        public override string ToString()
        {
            return "Name = " + Name + ", Age = " + Age;
        }
    }
}