namespace Munchausen.Metadata;

/// <summary>
/// Discovers immutable <see cref="ModelMetadata"/> for a model type. Isolating
/// discovery behind this seam keeps reflection in one place and lets a
/// source-generated implementation be substituted later (AOT is not promised in
/// v1.0).
/// </summary>
internal interface IModelMetadataProvider
{
    ModelMetadata GetMetadata(Type type);
}
