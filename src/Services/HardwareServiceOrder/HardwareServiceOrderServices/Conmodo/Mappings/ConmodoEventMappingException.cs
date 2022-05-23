using System.Runtime.Serialization;

namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    /// <summary>
    ///     A Conmodo specific exception that is thrown when the event-mapping requires the solution to change to the historical mapping implementation.
    /// </summary>
    [Serializable]
    public class ConmodoEventMappingException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConmodoEventMappingException"/> class.
        /// </summary>
        public ConmodoEventMappingException() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConmodoEventMappingException"/> class with a specified error message.
        /// </summary>
        /// <inheritdoc cref="Exception(string?)"/>
        public ConmodoEventMappingException(string? message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConmodoEventMappingException"/> class with a specified error message and a reference to
        ///     the inner exception that is the cause of this exception.
        /// </summary>
        /// <inheritdoc cref="Exception(string?, Exception?)"/>
        public ConmodoEventMappingException(string? message, Exception? innerException) : base(message, innerException) { }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ConmodoEventMappingException"/> class with serialized data.
        /// </summary>
        /// <inheritdoc cref="Exception(SerializationInfo, StreamingContext)"/>
        protected ConmodoEventMappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
