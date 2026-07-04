namespace Tulo.SigningPdf.Exceptions;

/// <summary>
/// StartupException
/// </summary>
public class StartupException : Exception
{
    /// <summary>
    /// StartupException
    /// </summary>
    public StartupException() { }
    /// <summary>
    /// StartupException
    /// </summary>
    /// <param name="message"></param>
    public StartupException(string message) : base(message) { }
    /// <summary>
    /// StartupException
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public StartupException(string message, Exception inner) : base(message, inner) { }
}
