using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Munchausen
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class ModelGenerator<T> : ValuesGenerator<T>
    {
        private readonly List<Func<T, T>> _functions = new List<Func<T, T>>();
        private bool _executed = false;

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
    }
}
