using System;

namespace Munchausen.Engine
{
    public class ObjectGenerator<T> where T : new()
    {
        public T Generate()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
