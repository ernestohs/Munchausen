namespace Munchausen.Inference;

/// <summary>
/// Normalizes a member or hint name per the catalog's matching rules: split on
/// case transitions, underscores, and hyphens, lowercase, and join. Because the
/// tokens are rejoined with no separator, this is equivalent to stripping
/// underscores/hyphens and lowercasing — <c>FirstName</c>, <c>first_name</c>, and
/// <c>FIRST-NAME</c> all normalize to <c>firstname</c>.
/// </summary>
internal static class MemberNameNormalizer
{
    public static string Normalize(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Span<char> buffer = name.Length <= 128 ? stackalloc char[name.Length] : new char[name.Length];
        int length = 0;
        foreach (char c in name)
        {
            if (c is '_' or '-')
            {
                continue;
            }

            buffer[length++] = char.ToLowerInvariant(c);
        }

        return new string(buffer[..length]);
    }
}
