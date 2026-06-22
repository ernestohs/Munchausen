using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Munchausen.Metadata;

/// <summary>
/// Builds get/set delegates with <see cref="Expression"/>.Compile, falling back
/// to reflection invocation when dynamic code is unsupported (NativeAOT). Init-only
/// setters are invoked through their set accessor, which is legal at runtime even
/// though plain C# forbids it after construction.
/// </summary>
internal sealed class CompiledExpressionAccessorFactory : IMemberAccessorFactory
{
    private readonly bool _useReflectionFallback;

    public CompiledExpressionAccessorFactory()
        : this(!RuntimeFeature.IsDynamicCodeSupported)
    {
    }

    internal CompiledExpressionAccessorFactory(bool useReflectionFallback)
        => _useReflectionFallback = useReflectionFallback;

    public MemberAccessor CreateAccessor(MemberMetadata member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (member.Member is not PropertyInfo property)
        {
            throw new NotSupportedException(
                $"Only property members are supported in v1.0; got {member.Member.GetType().Name}.");
        }

        return _useReflectionFallback
            ? CreateReflectionAccessor(property)
            : CreateCompiledAccessor(property);
    }

    private static MemberAccessor CreateReflectionAccessor(PropertyInfo property)
    {
        Func<object, object?>? getter = null;
        if (property.GetMethod is { IsPublic: true })
        {
            getter = instance => property.GetValue(instance);
        }

        Action<object, object?>? setter = null;
        if (property.SetMethod is { IsPublic: true })
        {
            setter = (instance, value) => property.SetValue(instance, value);
        }

        return new MemberAccessor(getter, setter);
    }

    private static MemberAccessor CreateCompiledAccessor(PropertyInfo property)
    {
        Type declaringType = property.DeclaringType!;
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");

        Func<object, object?>? getter = null;
        if (property.GetMethod is { IsPublic: true })
        {
            UnaryExpression access = Expression.Convert(
                Expression.Property(Expression.Convert(instance, declaringType), property),
                typeof(object));
            getter = Expression.Lambda<Func<object, object?>>(access, instance).Compile();
        }

        Action<object, object?>? setter = null;
        if (property.SetMethod is { IsPublic: true } setMethod)
        {
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            MethodCallExpression call = Expression.Call(
                Expression.Convert(instance, declaringType),
                setMethod,
                Expression.Convert(value, property.PropertyType));
            setter = Expression.Lambda<Action<object, object?>>(call, instance, value).Compile();
        }

        return new MemberAccessor(getter, setter);
    }
}
