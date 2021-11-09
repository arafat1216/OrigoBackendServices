using Common.Seedwork;
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
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updateLocation.CreatedBy;
        }

        public void PatchLocation(Location updateLocation)
        {
            bool isUpdated = false;
            if (Name != updateLocation.Name && updateLocation.Name != null)
            {
                Name = updateLocation.Name;
                isUpdated = true;
            }
            if (Description != updateLocation.Description && updateLocation.Description != null)
            {
                Description = updateLocation.Description;
                isUpdated = true;
            }
            if (Address1 != updateLocation.Address1 && updateLocation.Address1 != null)
            {
                Address1 = updateLocation.Address1;
                isUpdated = true;
            }
            if (Address2 != updateLocation.Address2 && updateLocation.Address2 != null)
            {
                Address2 = updateLocation.Address2;
                isUpdated = true;
            }
            if (PostalCode != updateLocation.PostalCode && updateLocation.PostalCode != null)
            {
                PostalCode = updateLocation.PostalCode;
                isUpdated = true;
            }
            if (City != updateLocation.City && updateLocation.City != null)
            {
                City = updateLocation.City;
                isUpdated = true;
            }
            if (Country != updateLocation.Country && updateLocation.Country != null)
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
