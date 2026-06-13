using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Munchausen.Metadata;

/// <summary>
/// Reflection-based <see cref="IModelMetadataProvider"/>. Results are immutable
/// and cached process-wide (pure derived data, so the no-mutable-globals rule
/// holds). Member order is fixed by <see cref="MemberInfo.MetadataToken"/>, which
/// matches source declaration order for a given compiled assembly — reordering
/// model source therefore changes seeded output, by design.
/// </summary>
internal sealed class ReflectionModelMetadataProvider : IModelMetadataProvider
{
    private const string IsExternalInitFullName = "System.Runtime.CompilerServices.IsExternalInit";
    private const string NullableContextAttributeFullName =
        "System.Runtime.CompilerServices.NullableContextAttribute";

    /// <summary>Process-wide instance whose cache is shared by all definitions and the automatic path.</summary>
    public static ReflectionModelMetadataProvider Shared { get; } = new();

    private readonly ConcurrentDictionary<Type, ModelMetadata> _cache = new();

    public ModelMetadata GetMetadata(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return _cache.GetOrAdd(type, BuildMetadata);
    }

    private static ModelMetadata BuildMetadata(Type type)
    {
        // NullabilityInfoContext is not thread-safe; one per build call.
        var nullability = new NullabilityInfoContext();

        PropertyInfo[] properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static property => property.GetIndexParameters().Length == 0)
            .OrderBy(static property => property.MetadataToken)
            .ToArray();

        var members = new MemberMetadata[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
            members[i] = BuildMember(properties[i], i, nullability);
        }

        ConstructorMetadata[] constructors = type
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(static constructor => constructor.MetadataToken)
            .Select(constructor => BuildConstructor(constructor, nullability))
            .ToArray();

        return new ModelMetadata(
            type,
            constructors,
            members,
            DetectNullableContext(type, members));
    }

    private static MemberMetadata BuildMember(PropertyInfo property, int order, NullabilityInfoContext nullability)
    {
        MemberWritability writability = GetWritability(property);
        NullabilityKind kind = GetNullability(property.PropertyType, nullability.Create(property).ReadState);
        Attribute[] attributes = property.GetCustomAttributes(inherit: true).Cast<Attribute>().ToArray();

        return new MemberMetadata(
            property,
            property.PropertyType,
            property.Name,
            writability,
            kind,
            IsRequired(property),
            attributes,
            order);
    }

    private static ConstructorMetadata BuildConstructor(ConstructorInfo constructor, NullabilityInfoContext nullability)
    {
        ParameterMetadata[] parameters = constructor.GetParameters()
            .Select(parameter => new ParameterMetadata(
                parameter,
                parameter.ParameterType,
                parameter.Name ?? string.Empty,
                GetNullability(parameter.ParameterType, nullability.Create(parameter).WriteState),
                parameter.GetCustomAttributes(inherit: true).Cast<Attribute>().ToArray()))
            .ToArray();

        return new ConstructorMetadata(constructor, parameters);
    }

    private static MemberWritability GetWritability(PropertyInfo property)
    {
        MethodInfo? setter = property.SetMethod;
        if (setter is null || !setter.IsPublic)
        {
            return MemberWritability.ReadOnly;
        }

        bool isInitOnly = setter.ReturnParameter
            .GetRequiredCustomModifiers()
            .Any(static modifier => modifier.FullName == IsExternalInitFullName);

        return isInitOnly ? MemberWritability.InitOnly : MemberWritability.Writable;
    }

    private static NullabilityKind GetNullability(Type type, NullabilityState state)
    {
        if (type.IsValueType)
        {
            return Nullable.GetUnderlyingType(type) is not null
                ? NullabilityKind.Nullable
                : NullabilityKind.NonNullable;
        }

        return state switch
        {
            NullabilityState.NotNull => NullabilityKind.NonNullable,
            NullabilityState.Nullable => NullabilityKind.Nullable,
            _ => NullabilityKind.Oblivious,
        };
    }

    private static bool IsRequired(PropertyInfo property) =>
        property.IsDefined(typeof(RequiredMemberAttribute), inherit: false)
        || property.IsDefined(typeof(RequiredAttribute), inherit: true);

    private static bool DetectNullableContext(Type type, IReadOnlyList<MemberMetadata> members)
    {
        foreach (CustomAttributeData attribute in type.GetCustomAttributesData())
        {
            if (attribute.AttributeType.FullName != NullableContextAttributeFullName)
            {
                continue;
            }

            // Flag byte: 0 = oblivious, 1 = not-annotated (non-null), 2 = annotated.
            return attribute.ConstructorArguments is [{ Value: byte flag }, ..] ? flag != 0 : true;
        }

        // No explicit context attribute: infer from members.
        bool anyReference = members.Any(static member => !member.ValueType.IsValueType);
        return !anyReference
            || members.Any(static member =>
                !member.ValueType.IsValueType && member.Nullability != NullabilityKind.Oblivious);
    }
}
