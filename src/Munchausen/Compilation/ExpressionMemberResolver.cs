using System.Linq.Expressions;
using System.Reflection;

namespace Munchausen.Compilation;

/// <summary>
/// The single place that enforces the v1.0 member-targeting rule: an expression
/// must be exactly <c>x =&gt; x.Property</c> (optionally with the compiler's
/// boxing <c>Convert</c> around a value-type member). Anything else (nested
/// paths, method calls, indexers, a non-parameter root) is rejected with LIE001.
/// </summary>
internal sealed class ExpressionMemberResolver
{
    public const string InvalidMemberExpressionCode = "LIE001";

    public ExpressionResolution Resolve(LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        Expression body = expression.Body;

        // Unwrap the Convert/ConvertChecked the compiler inserts when a value-type
        // member is accessed through Expression<Func<T, object>> or similar.
        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } convert)
        {
            body = convert.Operand;
        }

        if (body is MemberExpression { Member: PropertyInfo property } member
            && member.Expression is ParameterExpression parameter
            && expression.Parameters.Count == 1
            && ReferenceEquals(parameter, expression.Parameters[0]))
        {
            return ExpressionResolution.Resolved(property);
        }

        return ExpressionResolution.Failed(
            InvalidMemberExpressionCode,
            $"Invalid member expression '{expression.Body}'. " +
            "Expected a single member access such as 'x => x.Property'.");
    }
}

/// <summary>The result of resolving a member-targeting expression.</summary>
internal readonly struct ExpressionResolution
{
    private ExpressionResolution(bool isResolved, PropertyInfo? member, string? code, string? message)
    {
        IsResolved = isResolved;
        Member = member;
        DiagnosticCode = code;
        Message = message;
    }

    public bool IsResolved { get; }

    public PropertyInfo? Member { get; }

    public string? DiagnosticCode { get; }

    public string? Message { get; }

    public static ExpressionResolution Resolved(PropertyInfo member) =>
        new(true, member, null, null);

    public static ExpressionResolution Failed(string code, string message) =>
        new(false, null, code, message);
}
