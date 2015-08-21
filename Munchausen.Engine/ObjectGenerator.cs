using System.IO;

namespace Munchausen.Engine
{
    public class ObjectGenerator<T> where T : new()
    {
        public T Generate()
        {
            return new T();
        }
    }
}