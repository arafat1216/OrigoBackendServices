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
        // Todo: Set attributes and access modifiers
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
        public DateTime CreatedAt{ get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        public Location(Guid callerId, string name, string description, string address1, string address2, string postalCode, string city, string country)
        {
            LocationId = Guid.NewGuid();
            Name = name;
            Description = description;
            Address1 = address1;
            Address2 = address2;
            PostalCode = postalCode;
            City = city;
            Country = country;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }
    }

}
