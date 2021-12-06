namespace ProductCatalog.Domain.Exceptions
{
    [Serializable]
    public class RequirementNotFulfilledException : Exception
    {
        public RequirementNotFulfilledException() { }

        public RequirementNotFulfilledException(string message) : base(message) { }

        public RequirementNotFulfilledException(string message, Exception inner) : base(message, inner) { }

        protected RequirementNotFulfilledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
