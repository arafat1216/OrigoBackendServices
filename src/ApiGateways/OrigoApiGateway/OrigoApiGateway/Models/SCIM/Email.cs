using AutoMapper;
using Google.Api;
using Humanizer;

namespace OrigoApiGateway.Models.SCIM;

/// <summary>
///     Email addresses for the User.  The value SHOULD be specified
///     according to[RFC5321].  Service providers SHOULD canonicalize the
///     value according to[RFC5321], e.g., "bjensen@example.com" instead
///     of "bjensen@EXAMPLE.COM".  The "display" sub-attribute MAY be used
///     to return the canonicalized representation of the email value.
///     The "type" sub-attribute is used to provide a classification
///     meaningful to the(human) user.The user interface should
///     encourage the use of basic values of "work", "home", and "other"
///     and MAY allow additional type values to be used at the discretion
///     of SCIM clients.
/// </summary>
public class Email
{
    /// <summary>
    ///     The email address, e.g. "bjensen@example.com"
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     The type of email address, e.g. "work", "home"
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     A user may have one primary email.
    ///     This should correspond to the username/email of the user in the database and Okta.
    /// </summary>
    public bool Primary { get; set; }
}
