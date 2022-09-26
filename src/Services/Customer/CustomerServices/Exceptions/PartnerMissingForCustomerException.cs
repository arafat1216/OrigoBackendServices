using System;

namespace CustomerServices.Exceptions;

public class PartnerMissingForCustomerException : Exception
{
    public PartnerMissingForCustomerException()
    {
    }

    public PartnerMissingForCustomerException(string message) : base(message)
    {
    }

    public PartnerMissingForCustomerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}