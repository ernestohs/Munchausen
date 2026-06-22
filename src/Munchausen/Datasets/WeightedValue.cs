namespace Munchausen;

/// <summary>A value paired with a relative weight for <see cref="RandomData.Weighted{T}"/>.</summary>
/// <typeparam name="T">The value type.</typeparam>
/// <param name="Value">The value.</param>
/// <param name="Weight">The relative weight; must be finite and positive.</param>
public readonly record struct WeightedValue<T>(T Value, double Weight);
