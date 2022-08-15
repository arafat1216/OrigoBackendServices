namespace HardwareServiceOrderServices.Exceptions
{
    /// <summary>
    ///     Thrown when a request was aborted because one or more provided values was not found during a database lookup.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        public NotFoundException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message"> The message that describes the error. </param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error 
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception. </param>
        /// <param name="innerException">  The exception that is the cause of the current exception, or a <see langword="null"/> reference
        ///     if no inner exception is specified. </param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
