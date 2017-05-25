using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Munchausen
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public sealed class Lie<T>
    {
        private static readonly Lazy<Lie<T>> Lazy = new Lazy<Lie<T>>(() => new Lie<T>());

        public static Lie<T> Instance => Lazy.Value;

        private static readonly Func<Expression, object> ArgumentsFromExpression =
            argument => Expression.Lambda(argument).Compile().DynamicInvoke();

        private static readonly Dictionary<Type, Func<int, object>> TypeHandlers = new Dictionary<Type, Func<int, object>>
        {
            {typeof(int), i => i }
        };

        private static object[] ConstructorArguments { get; set; }

        private Lie()
        {
        }

        public Lie<T> WithConstructor(Expression<Func<T>> constructor)
        {
            if (constructor.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException("WithConstructor expects a constructor expression");
            }

            var arguments = ((NewExpression) constructor.Body).Arguments;

            ConstructorArguments = arguments.Select(ArgumentsFromExpression).ToArray<object>();

            return Instance;
        }

        public static T Generate()
        {
            return CreateInstance();
        }

        // public 

        private static T CreateInstance(int index = 0)
        {
            T instance;
            if (ConstructorArguments == null)
            {
                instance = Activator.CreateInstance<T>();
            }
            else
            {
                instance = (T)Activator.CreateInstance(typeof(T), ConstructorArguments);
            }

            Parallel.ForEach(typeof(T).GetProperties(), DynamicSetValue(instance, index));

            return instance;
        }

        private static Action<PropertyInfo> DynamicSetValue(T instance, int index)
        {
            return fieldInfo => fieldInfo.SetValue(instance, GenerateValue(fieldInfo.PropertyType, index));
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static object GenerateValue(Type type, int index)
        {
            if (TypeHandlers.ContainsKey(type))
            {
                return TypeHandlers[type](index);
            }

            return default(Type);
        }

        public static IList<T> Generate(int number)
        {
            var results = new List<T>();

            for (var i = 0; i < number; i++)
            {
                results.Add(CreateInstance(i));
            }

            return results;
        }
    }
}