using System.Runtime.Serialization;

namespace ProductCatalog.Domain.Exceptions
{
    /// <summary>
    ///     Represents an error that occurs when a entity is not found.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <inheritdoc/>
        public EntityNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error message.
        /// </summary>
        ///         /// <inheritdoc/>
        public EntityNotFoundException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <inheritdoc/>
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class with serialized data.
        /// </summary>
        /// <inheritdoc/>
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
