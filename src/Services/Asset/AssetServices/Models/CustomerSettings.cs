using AssetServices.DomainEvents;
using AssetServices.Exceptions;
using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {
        public CustomerSettings(Guid customerId, IReadOnlyCollection<LifeCycleSetting>? lifeCycleSettings = null, DisposeSetting? disposeSetting = null)
        {
            CustomerId = customerId;
            if (lifeCycleSettings != null)
            {
                _lifeCycleSettings.AddRange(lifeCycleSettings);
            }

            if (disposeSetting != null)
            {
                DisposeSetting = disposeSetting;
            }
        }

        public CustomerSettings() { }

        public CustomerSettings(Guid customerId, Guid callerId)
        {
            CustomerId = customerId;
            _lifeCycleSettings = new List<LifeCycleSetting>();
            AddDomainEvent(new CustomerSettingsCreatedDomainEvent<CustomerSettings>(this, callerId));
        }

        public Guid CustomerId { get; protected set; }

        private readonly List<LifeCycleSetting> _lifeCycleSettings = new();
        [JsonIgnore]
        public IReadOnlyCollection<LifeCycleSetting> LifeCycleSettings => _lifeCycleSettings.AsReadOnly();

        public DisposeSetting? DisposeSetting { get; private set; }

        public void AddLifeCycleSetting(LifeCycleSetting lifeCycleSetting, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            _lifeCycleSettings.Add(lifeCycleSetting);
        }
        public void AddDisposeSetting(DisposeSetting disposeSetting, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            DisposeSetting = disposeSetting;
        }


    }
}
