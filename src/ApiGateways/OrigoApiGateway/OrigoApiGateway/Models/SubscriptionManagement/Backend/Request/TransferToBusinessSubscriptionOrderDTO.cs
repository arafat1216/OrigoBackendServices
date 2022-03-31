﻿using System;
using System.Collections.Generic;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;

public record TransferToBusinessSubscriptionOrderDTO
{
    /// <summary>
    ///     The current owner the subscription will be transferred from.
    /// </summary>
    public PrivateSubscription? PrivateSubscription { get; set; }

    public BusinessSubscription? BusinessSubscription { get; set; }

    /// <summary>
    ///     The mobile number to be transferred
    /// </summary>
    public string MobileNumber { get; set; }
    /// <summary>
    /// The operator id they get from the business subscription
    /// </summary>
    public int OperatorId { get; set; }

    /// <summary>
    ///     New operator account identifier
    /// </summary>
    public int? OperatorAccountId { get; set; }

    /// <summary>
    ///     Customer Subscription product identifier
    /// </summary>
    public int SubscriptionProductId { get; set; }

    /// <summary>
    ///     Data package name
    /// </summary>
    public string DataPackage { get; set; }

    /// <summary>
    ///     SIM card number
    /// </summary>
    public string? SIMCardNumber { get; set; }

    /// <summary>
    ///     SIM card number
    /// </summary>
    public string SIMCardAction { get; set; }
    /// <summary>
    ///     SIM card reciver address
    /// </summary>
    public SimCardAddress? SimCardAddress { get; set; } = null;


    /// <summary>
    ///     Date of transfer
    /// </summary>
    public DateTime OrderExecutionDate { get; set; }

    /// <summary>
    ///     List of add on products to the subscription
    /// </summary>
    public IList<string> AddOnProducts { get; set; } = new List<string>();

    public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();
    /// <summary>
    ///     A phone number reference to the operator account
    /// </summary>
    public string? OperatorAccountPhoneNumber { get; set; }
    public NewOperatorAccountRequested? NewOperatorAccount { get; set; }

    public Guid CallerId { get; set; }
}