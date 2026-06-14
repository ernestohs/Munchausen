namespace Munchausen;

/// <summary>Base type for all Munchausen exceptions.</summary>
public abstract class LieException : Exception
{
    private protected LieException(string message)
        : base(message)
    {
    }

    private protected LieException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
