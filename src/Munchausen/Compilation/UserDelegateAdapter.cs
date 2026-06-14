using System.Linq.Expressions;

namespace Munchausen.Compilation;

/// <summary>
/// Wraps typed user delegates into the uniform boxed shapes the runtime invokes,
/// compiled once at build time. Invoking the typed delegate directly (not via
/// DynamicInvoke) keeps the user's exception as the real <c>InnerException</c>.
/// </summary>
internal static class UserDelegateAdapter
{
    /// <summary>Adapts <c>Func&lt;GenerationContext, TProperty&gt;</c> to <c>Func&lt;GenerationContext, object?&gt;</c>.</summary>
    public static Func<GenerationContext, object?> Value(Delegate typedGenerator)
    {
        ParameterExpression context = Expression.Parameter(typeof(GenerationContext), "context");
        UnaryExpression body = Expression.Convert(
            Expression.Invoke(Expression.Constant(typedGenerator), context), typeof(object));
        return Expression.Lambda<Func<GenerationContext, object?>>(body, context).Compile();
    }

    /// <summary>Adapts <c>Func&lt;GenerationContext, T, TProperty&gt;</c> to <c>Func&lt;GenerationContext, object, object?&gt;</c>.</summary>
    public static Func<GenerationContext, object, object?> Derive(Delegate typedGenerator, Type modelType)
    {
        ParameterExpression context = Expression.Parameter(typeof(GenerationContext), "context");
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        UnaryExpression body = Expression.Convert(
            Expression.Invoke(
                Expression.Constant(typedGenerator),
                context,
                Expression.Convert(instance, modelType)),
            typeof(object));
        return Expression.Lambda<Func<GenerationContext, object, object?>>(body, context, instance).Compile();
    }
}
