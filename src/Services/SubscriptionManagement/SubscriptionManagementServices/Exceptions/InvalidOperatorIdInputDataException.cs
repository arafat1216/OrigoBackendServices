using System.Runtime.Serialization;

namespace SubscriptionManagementServices.Exceptions;

[Serializable]
public class InvalidOperatorIdInputDataException : SubscriptionManagementException
{
    public InvalidOperatorIdInputDataException()
    {
    }

    public InvalidOperatorIdInputDataException(Exception exception)
    {
    }

    public InvalidOperatorIdInputDataException(string message) : base(message)
    {
    }

    public InvalidOperatorIdInputDataException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidOperatorIdInputDataException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }
}