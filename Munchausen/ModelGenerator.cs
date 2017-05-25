using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Munchausen
{
    public class ModelGenerator<T>
    {
        private T _target;
        private readonly long _index;
        private readonly List<Func<T, T>> _functions = new List<Func<T, T>>();
        private bool _executed = false;

        private static readonly Dictionary<Type, Func<long, object>> TypeHandlers = new Dictionary<Type, Func<long, object>>
        {
            {typeof(int), i => (int)i }
        };

        public ModelGenerator(T target, long index)
        {
            _target = target;
            _index = index;
        }

        public ModelGenerator(T target, List<Func<T, T>> funcs)
        {
            _target = target;
            _functions = funcs;
        }

        public ModelGenerator<T> AddFunction(Func<T, T> function)
        {
            _functions.Add(function);

            return this;
        }

        public ModelGenerator<T> AddFunctions(List<Func<T, T>> function)
        {
            _functions.AddRange(function);

            return this;
        }

        public ModelGenerator<T> ApplyFunctions()
        {
            foreach (var function in _functions)
            {
                _target = function(_target);
            }

            _executed = true;
            return this;
        }

        public T GetInstance()
        {
            if (_executed) return _target;

            if (_functions.Count > 0)
            {
                this.ApplyFunctions();
            }
            else
            {
                AutoFillFields();
                _executed = true;
            }

            return _target;
        }

        private void AutoFillFields()
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
