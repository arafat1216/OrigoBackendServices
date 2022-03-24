﻿using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;

#nullable enable

namespace CustomerServices.Models
{
    public class Location : Entity
    {
        public Guid LocationId { get; protected set; }
        public string Name { get; protected set; }
        public string? Description { get; protected set; }
        public string? Address1 { get; protected set; }
        public string? Address2 { get; protected set; }
        public string? PostalCode { get; protected set; }
        public string? City { get; protected set; }
        public string? Country { get; protected set; }

        /// <summary>
        /// Added to prevent Entity framework No suitable constructor found exception.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Location()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        { }

        public Location(Guid locationId, Guid callerId, string name, string description, string address1, string address2, string postalCode, string city, string country)
        {
            LocationId = locationId;
            Name = name;
            Description = description;
            Address1 = address1;
            Address2 = address2;
            PostalCode = postalCode;
            City = city;
            Country = country;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            AddDomainEvent(new LocationCreatedDomainEvent(this));
        }

        /// <summary>
        /// Cannot check for null in constructor, since patch method needs to check if Location object to update has fields we wish to ignore
        /// This method allows us to set null fields to String.Empty for update method, while ignoring null fields for patch method.
        /// </summary>
        public void SetFieldsToEmptyIfNull()
        {
            if (Name == null) Name = "";
            if (Description == null) Description = "";
            if (Address1 == null) Address1 = "";
            if (Address2 == null) Address2 = "";
            if (PostalCode == null) PostalCode = "";
            if (City == null) City = "";
            if (Country == null) Country = "";
        }

        public void UpdateLocation(Location updateLocation)
        {
            Name = (updateLocation.Name == null) ? "" : updateLocation.Name;
            Description = (updateLocation.Description == null) ? "" : updateLocation.Description;
            Address1 = (updateLocation.Address1 == null) ? "" : updateLocation.Address1;
            Address2 = (updateLocation.Address2 == null) ? "" : updateLocation.Address2;
            PostalCode = (updateLocation.PostalCode == null) ? "" : updateLocation.PostalCode;
            City = (updateLocation.City == null) ? "" : updateLocation.City;
            Country = (updateLocation.Country == null) ? "" : updateLocation.Country;
            UpdatedBy = updateLocation.CreatedBy;
            AddDomainEvent(new LocationUpdatedDomainEvent(this));
        }

        public void PatchLocation(Location updateLocation)
        {
            bool isUpdated = false;
            if (Name != updateLocation.Name && updateLocation.Name != null)
            {
                string oldName = Name;
                Name = updateLocation.Name;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateNameDomainEvent(this, oldName));
            }
            if (Description != updateLocation.Description && updateLocation.Description != null)
            {
                string OldDescription = Description;
                Description = updateLocation.Description;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateDescriptionDomainEvent(this,OldDescription));
            }
            if (Address1 != updateLocation.Address1 && updateLocation.Address1 != null)
            {
                string OldAddress1 = Address1;
                Address1 = updateLocation.Address1;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateAddressDomainEvent(this,1, OldAddress1));
            }
            if (Address2 != updateLocation.Address2 && updateLocation.Address2 != null)
            {
                string OldAddress2 = Address2;
                Address2 = updateLocation.Address2;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateAddressDomainEvent(this,2,OldAddress2));
            }
            if (PostalCode != updateLocation.PostalCode && updateLocation.PostalCode != null)
            {
                string OldPostalCode = PostalCode;
                PostalCode = updateLocation.PostalCode;
                isUpdated = true;
                AddDomainEvent(new LocationUpdatePostalCodeDomainEvent(this, OldPostalCode));
            }
            if (City != updateLocation.City && updateLocation.City != null)
            {
                string OldCity = City;
                City = updateLocation.City;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateCityDomainEvent(this,OldCity));
            }
            if (Country != updateLocation.Country && updateLocation.Country != null)
            {
                string OldCountry = Country;
                Country = updateLocation.Country;
                isUpdated = true;
                AddDomainEvent(new LocationUpdateCountryDomainEvent(this, OldCountry)); 
            }
            if (isUpdated)
            {
                LastUpdatedDate = DateTime.UtcNow;
                UpdatedBy = updateLocation.CreatedBy;
            }
        }

        public void Delete(Guid callerId)
        {
            IsDeleted = true;
            LastUpdatedDate = DateTime.UtcNow;
            DeletedBy = callerId;
            AddDomainEvent(new LocationDeletedDomainEvent(this));
        }
    }
}
