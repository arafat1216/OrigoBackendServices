namespace Common.Exceptions;

/// <summary>
///     Represents a error caused by a missing or invalid HTTP-Header.
/// </summary>
public class InvalidSaveMethodException : Exception
{
    /// <summary>
    ///     Throws an exception if saving to the dbcontext is not done with an async method.
    /// </summary>
    /// <param name="message"> The error message that explains the reason for the exception. </param>
    public InvalidSaveMethodException(string message) : base(message)
    {
    }


    /// <summary>
    ///     Throws an exception if saving to the dbcontext is not done with an async method.
    /// </summary>
    /// <param name="message"> The error message that explains the reason for the exception. </param>
    /// <param name="innerException"></param>
    public InvalidSaveMethodException(string message, Exception innerException) : base(message, innerException)
    {
    }
}