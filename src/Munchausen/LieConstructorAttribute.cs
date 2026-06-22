namespace Munchausen;

/// <summary>
/// Marks the constructor Munchausen should use when constructing the model,
/// overriding automatic constructor selection.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class LieConstructorAttribute : Attribute;
