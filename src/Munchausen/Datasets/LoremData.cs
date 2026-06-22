using System.Text;
using Munchausen.Datasets;
using Munchausen.Runtime;

namespace Munchausen;

/// <summary>Classic lorem-style filler text. Never collides with name/address data.</summary>
public sealed class LoremData
{
    private readonly DeterministicRandom _random;

    internal LoremData(DeterministicRandom random) => _random = random;

    /// <summary>A single filler word.</summary>
    public string Word() => _random.Pick(EnData.LoremWords);

    /// <summary><paramref name="count"/> space-joined filler words.</summary>
    public string Words(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            builder.Append(Word());
        }

        return builder.ToString();
    }

    /// <summary>A capitalized, period-terminated sentence of 6 to 12 words.</summary>
    public string Sentence()
    {
        string body = Words(_random.Int(6, 12));
        return $"{char.ToUpperInvariant(body[0])}{body[1..]}.";
    }

    /// <summary>A paragraph of 3 to 6 sentences.</summary>
    public string Paragraph()
    {
        int sentences = _random.Int(3, 6);
        var builder = new StringBuilder();
        for (int i = 0; i < sentences; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            builder.Append(Sentence());
        }

        return builder.ToString();
    }

    /// <summary>
    /// A stream of words truncated at a word boundary so the result never exceeds
    /// <paramref name="maxLengthInclusive"/> and never cuts a word.
    /// </summary>
    public string Text(int maxLengthInclusive)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxLengthInclusive, 1);

        var builder = new StringBuilder();
        while (true)
        {
            string word = Word();
            int addition = builder.Length == 0 ? word.Length : word.Length + 1;
            if (builder.Length + addition > maxLengthInclusive)
            {
                break;
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(word);
        }

        return builder.ToString();
    }
}
