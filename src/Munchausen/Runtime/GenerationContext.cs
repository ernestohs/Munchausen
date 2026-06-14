namespace Munchausen;

/// <summary>
/// The façade passed to user rules during generation, exposing the operation's
/// randomness, datasets, reference time, and services. Its members are added in
/// later milestones; it is declared here because builder delegates reference it.
/// A context is valid only during the generation call that supplied it.
/// </summary>
public sealed class GenerationContext
{
    internal GenerationContext()
    {
    }
}
