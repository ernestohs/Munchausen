using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Munchausen
{
    public class ValuesGenerator<T>
    {
        protected T _target;
        protected long _index;

        private static readonly Dictionary<Type, Func<long, object>> TypeHandlers = new Dictionary<Type, Func<long, object>>
        {
            {typeof(int), i => (int)i }
        };

        protected void AutoFillFields()
        {
            Parallel.ForEach(typeof(T).GetProperties(), DynamicSetValue);
        }

        private void DynamicSetValue(PropertyInfo propertyInfo, ParallelLoopState state)
        {
            propertyInfo.SetValue(_target, GenerateSequencialValue(propertyInfo, _index));
        }

        private object GenerateSequencialValue(PropertyInfo propertyInfo, long index)
        {
            Type type = propertyInfo.PropertyType;
            var info = type.GetTypeInfo();

            if (info.IsValueType)
            {

                if (TypeHandlers.ContainsKey(type))
                {
                    return TypeHandlers[type](index);
                }

                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}