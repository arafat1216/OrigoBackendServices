namespace Common.Extensions;

public static class GuidExtension
{
    public static Guid SystemUserId(this Guid guid)
    {
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }

    /// <summary>
    /// System user for the customer data sync methods.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static Guid PubsubUserId(this Guid guid)
    {
        return Guid.Parse("00000000-0000-0000-0000-000000000002");
    }

    /// <summary>
    /// System user for the Okta SCIM integration.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static Guid ScimUserId(this Guid guid)
    {
        return Guid.Parse("00000000-0000-0000-0000-000000000003");
    }
    /// <summary>
    /// System user for the EoL Scheduler Service.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static Guid EoLUserId(this Guid guid)
    {
        return Guid.Parse("00000000-0000-0000-0000-000000000004");
    }

}