using System;
using Common.Enums;

namespace Common.Model;

/// <summary>
/// Details for one of the errors related to
/// an error response.
/// </summary>
public class ErrorResponseDetail
{
    public ErrorResponseDetail(OrigoErrorCodes code, string message, Guid trackingCode)
    {
        Code = code;
        Message = message;
        TrackingCode = trackingCode;
    }

    private ErrorResponseDetail() { }

    /// <summary>
    /// The error code used uniquely across the solutions.
    /// </summary>
    public  OrigoErrorCodes Code { get; protected set; }

    /// <summary>
    /// A message describing the error.
    /// </summary>
    public string Message { get; protected set; }

    /// <summary>
    /// A unique id given by the place in code where this happened.
    /// Must be created as a fixed GUID where this is created.
    /// </summary>
    public Guid TrackingCode { get; protected set; }
}