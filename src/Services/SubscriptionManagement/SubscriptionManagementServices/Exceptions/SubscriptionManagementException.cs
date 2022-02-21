﻿using System.Runtime.Serialization;

namespace SubscriptionManagementServices.Exceptions
{
    [Serializable]
    public class SubscriptionManagementException : Exception
    {

        public SubscriptionManagementException()
        {
        }

        public SubscriptionManagementException(Exception exception)
        {
        }

        public SubscriptionManagementException(string message) : base(message)
        {
        }

        public SubscriptionManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SubscriptionManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}