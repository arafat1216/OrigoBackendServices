namespace OrigoApiGateway.Exceptions;

public class ScimException : Exception
{
    public ScimException (ILogger logger)
    {
        logger.LogError(this, "SCIM error");

    }
    public ScimException(string message, ILogger logger) : base(message)
    {
        logger.LogError(this, message);
    }
}
