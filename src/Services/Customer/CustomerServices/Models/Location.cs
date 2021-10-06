﻿using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class Location : Entity
    {
        public Guid LocationId { get; protected set; }
        public Guid CreatedBy { get; protected set; }
        public Guid UpdatedBy { get; protected set; }

        public string Name { get; protected set; }
        public string? Description { get; protected set; }
        public string? Address1 { get; protected set; }
        public string? Address2 { get; protected set; }
        public string? PostalCode { get; protected set; }
        public string? City { get; protected set; }
        public string? Country { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        /// <summary>
        /// Added to prevent Entity framework No suitable constructor found exception.
        /// </summary>
        protected Location()
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
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public void UpdateLocation(Location updateLocation)
        {
            Name = updateLocation.Name;
            Description = updateLocation.Description;
            Address1 = updateLocation.Address1;
            Address2 = updateLocation.Address2;
            PostalCode = updateLocation.PostalCode;
            City = updateLocation.City;
            Country = updateLocation.Country;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updateLocation.CreatedBy;
        }

        public void PatchLocation(Location updateLocation)
        {
            bool isUpdated = false;
            if (Name != updateLocation.Name)
            {
                Name = updateLocation.Name;
                isUpdated = true;
            }
            if (Description != updateLocation.Description)
            {
                Description = updateLocation.Description;
                isUpdated = true;
            }
            if (Address1 != updateLocation.Address1)
            {
                Address1 = updateLocation.Address1;
                isUpdated = true;
            }
            if (Address2 != updateLocation.Address2)
            {
                Address2 = updateLocation.Address2;
                isUpdated = true;
            }
            if (PostalCode != updateLocation.PostalCode)
            {
                PostalCode = updateLocation.PostalCode;
                isUpdated = true;
            }
            if (City != updateLocation.City)
            {
                City = updateLocation.City;
                isUpdated = true;
            }
            if (Country != updateLocation.Country)
            {
                Country = updateLocation.Country;
                isUpdated = true;
            }
            if (isUpdated)
            {
                UpdatedAt = DateTime.UtcNow;
                UpdatedBy = updateLocation.CreatedBy;
            }
        }

        public void Delete(Guid callerId)
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = callerId;
        }
    }
}
