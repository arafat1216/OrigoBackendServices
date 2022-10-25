using System;
using System.Runtime.Serialization;

namespace OrigoApiGateway.Exceptions
{
    [Serializable]
    public class ProductCatalogException : Exception
    {

        public ProductCatalogException()
        {
        }

        public ProductCatalogException(Exception exception)
        {
        }

        public ProductCatalogException(string message) : base(message)
        {
        }

        public ProductCatalogException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProductCatalogException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
