namespace CiscoConfigurationFileManager.Models;

/// <summary>
/// Class representing a read-only wrapper around a string.
/// </summary>
public class StringWrapperRead
{
    public StringWrapperRead(string? s)
    {
        String = s;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringWrapperRead"/> class.
    /// </summary>
    /// <param name="s">The string to wrap.</param>
    public string? String { get; }
}