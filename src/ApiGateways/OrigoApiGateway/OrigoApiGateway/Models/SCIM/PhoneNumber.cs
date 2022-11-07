using AutoMapper.Execution;
using Google.Api;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using static Humanizer.In;

namespace OrigoApiGateway.Models.SCIM;

/// <summary>
///     Phone numbers for the user.  The value SHOULD be specified
///     according to the format defined in [RFC3966], e.g.,
///     'tel:+1-201-555-0123'.  Service providers SHOULD canonicalize the
///     value according to[RFC3966] format, when appropriate.The
///     "display" sub-attribute MAY be used to return the canonicalized
///     representation of the phone number value.  The sub-attribute
///     "type" often has typical values of "work", "home", "mobile",
///     "fax", "pager", and "other" and MAY allow more types to be defined
///     by the SCIM clients.
/// </summary>
public class PhoneNumber
{
    /// <summary>
    ///     The phone number, e.g. "+4799988777"
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     The type of email address, e.g. "work", "home"
    /// </summary>
    public string Type { get; set; }
}
