namespace Common.Exceptions;

/// <summary>
///     Represents a error caused by a missing or invalid HTTP-Header.
/// </summary>
public class HttpHeaderException : Exception
{
    /// <summary>
    /// The name of the HTTP-Header parameter that is missing or invalid.
    /// </summary>
    public string HttpHeaderParameter { get; }


    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpHeaderParameter"/> class with a specified error message.
    /// </summary>
    /// <param name="httpHeaderParameter"> The name of the HTTP-Header parameter that is missing or invalid. </param>
    /// <param name="message"> The error message that explains the reason for the exception. </param>
    public HttpHeaderException(string httpHeaderParameter, string message) : base(message)
    {
        HttpHeaderParameter = httpHeaderParameter;
    }


    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpHeaderParameter"/> class with a specified error message and a
    ///     reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="httpHeaderParameter"> The name of the HTTP-Header parameter that is missing or invalid. </param>
    /// <param name="message"> The error message that explains the reason for the exception. </param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or a null reference
    ///     (Nothing in Visual Basic) if no inner exception is specified. </param>
    public HttpHeaderException(string httpHeaderParameter, string message, Exception innerException) : base(message, innerException)
    {
        HttpHeaderParameter = httpHeaderParameter;
    }
}