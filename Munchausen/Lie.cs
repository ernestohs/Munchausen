using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Munchausen
{

    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public sealed class Lie<T>
    {
        private static readonly Lazy<Lie<T>> Lazy = new Lazy<Lie<T>>(() => new Lie<T>());

        public static Lie<T> Instance => Lazy.Value;

        private static readonly Func<Expression, object> ArgumentsFromExpression =
            argument => Expression.Lambda(argument).Compile().DynamicInvoke();
        
        private static object[] ConstructorArguments { get; set; }

        private Lie() { }

        public Lie<T> WithConstructor(Expression<Func<T>> constructor)
        {
            if (constructor.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException("WithConstructor expects a constructor expression");
            }

            var arguments = ((NewExpression) constructor.Body).Arguments;

            ConstructorArguments = arguments.Select(ArgumentsFromExpression).ToArray();

            return Instance;
        }

        private static T CreateInstance(long index = 0)
        {
            var instance = ConstructInstance();

            var transformator = new ModelGenerator<T>(instance, index);

            return transformator.GetInstance();
        }

        private static T ConstructInstance()
        {
            T instance;

            if (ConstructorArguments == null)
            {
                instance = Activator.CreateInstance<T>();
            }
            else
            {
                instance = (T) Activator.CreateInstance(typeof(T), ConstructorArguments);
            }

            return instance;
        }

        public static T Generate()
        {
            return CreateInstance();
        }

        public static IList<T> Generate(long number)
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