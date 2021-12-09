using System.Runtime.Serialization;

namespace ProductCatalog.Common.Exceptions
{
    /// <summary>
    ///     Represents errors that occur because if unfulfilled requirements.
    /// </summary>
    [Serializable]
    public class RequirementNotFulfilledException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementNotFulfilledException"/> class.
        /// </summary>
        /// <inheritdoc/>
        public RequirementNotFulfilledException() : base() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementNotFulfilledException"/> class with a specified error message.
        /// </summary>
        /// <inheritdoc/>
        public RequirementNotFulfilledException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementNotFulfilledException"/> class with a specified error message
        ///     and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <inheritdoc/>
        public RequirementNotFulfilledException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementNotFulfilledException"/> class with serialized data.
        /// </summary>
        /// <inheritdoc/>
        protected RequirementNotFulfilledException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
