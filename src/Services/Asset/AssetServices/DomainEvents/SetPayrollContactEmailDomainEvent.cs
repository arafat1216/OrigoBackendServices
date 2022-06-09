﻿using AssetServices.Models;
using Common.Logging;
using System;

namespace AssetServices.DomainEvents
{
    public class SetPayrollContactEmailDomainEvent : BaseEvent
    {
        public SetPayrollContactEmailDomainEvent(DisposeSetting disposeSetting, Guid callerId, string previousEmail) : base(
        disposeSetting.ExternalId)
        {
            DisposeSetting = disposeSetting;
            CallerId = callerId;
            PreviousEmail = previousEmail;
        }

        public DisposeSetting DisposeSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public string PreviousEmail { get; protected set; }

        public override string EventMessage()
        {
            return string.IsNullOrEmpty(PreviousEmail)?
                $"Dispose Setting for id {DisposeSetting.ExternalId}; Set 'Payroll Contact Email' to {DisposeSetting.PayrollContactEmail}."
                : $"Dispose Setting for id {DisposeSetting.ExternalId}; Changing 'Payroll Contact Email' from {PreviousEmail} to {DisposeSetting.PayrollContactEmail}.";
        }
    }
}