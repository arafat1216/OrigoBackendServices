﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using AssetServices.DomainEvents;
using AssetServices.DomainEvents.EndOfLifeCycleEvents;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Models
{
    [Owned]
    public class DisposeSetting : Entity
    {
        public DisposeSetting(string payrollContactEmail, Guid callerId)
        {
            CreatedBy = callerId;
            PayrollContactEmail = payrollContactEmail;
        }

        public DisposeSetting()
        {
        }

        /// <summary>
        ///     Email where notification will ben sent for payroll information.
        /// </summary>
        public string PayrollContactEmail { get; protected set; } = string.Empty;

        /// <summary>
        ///     The organization's Return locations/addresses.
        /// </summary>
        private readonly List<ReturnLocation> _returnLocations = new();
        [JsonIgnore]
        public IReadOnlyCollection<ReturnLocation> ReturnLocations => _returnLocations.AsReadOnly();

        /// <summary>
        ///     Return Drop Off location added for this Dispose Setting
        /// </summary>
        /// <param name="returnLocation">The ReturnLocation Data</param>
        /// <param name="callerId">The userid making this assignment</param>
        /// <param name="customerId">The customerId making this assignment For</param>
        public void AddReturnLocation(ReturnLocation returnLocation, Guid callerId, Guid customerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            _returnLocations.Add(returnLocation);
            AddDomainEvent(new ReturnLocationCreatedDomainEvent(returnLocation, callerId, customerId));
        }
        /// <summary>
        ///     Return Drop Off location removed for this Dispose Setting
        /// </summary>
        /// <param name="returnLocation">The ReturnLocation Data</param>
        /// <param name="callerId">The userid making this assignment</param>
        /// <param name="customerId">The customerId making this assignment For</param>
        public void RemoveReturnLocation(ReturnLocation returnLocation, Guid callerId, Guid customerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            _returnLocations.Remove(returnLocation);
            AddDomainEvent(new ReturnLocationRemovedDomainEvent(returnLocation, callerId, customerId));
        }

        /// <summary>
        ///     Return Drop Off location updated for this Dispose Setting
        /// </summary>
        /// <param name="returnLocation"Id>The Id of ReturnLocation that ill be updated</param>
        /// <param name="returnLocation">The ReturnLocation Data</param>
        /// <param name="callerId">The userid making this assignment</param>
        /// <param name="customerId">The customerId making this assignment For</param>
        public void UpdateReturnLocation(Guid callerId, Guid returnLocationId, ReturnLocation returnLocationData)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var returnLocation = _returnLocations.FirstOrDefault(x => x.ExternalId == returnLocationId);
            if (returnLocation is null)
                return;
            AddDomainEvent(new ReturnLocationUpdatedDomainEvent(returnLocation, callerId, returnLocationData));
            returnLocation.Update(returnLocationData);
        }
        /// <summary>
        ///     Set Payroll Contact Email for this customer.
        /// </summary>
        /// <param name="payrollContactEmail">The email for updating PayrollContactEmail</param>
        /// <param name="callerId">The userid making this assignment</param>
        public void SetPayrollContactEmail(string payrollContactEmail, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var previousEmail = PayrollContactEmail;
            PayrollContactEmail = payrollContactEmail;
            AddDomainEvent(new SetPayrollContactEmailDomainEvent(this, callerId, previousEmail));
        }
    }
}
