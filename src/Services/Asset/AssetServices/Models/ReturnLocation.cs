using Common.Seedwork;
using System;
using System.Collections.Generic;

namespace AssetServices.Models
{
    public class ReturnLocation : ValueObject
    {
        /// <summary>
        ///     The entities internal database ID.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///     The external uniquely identifying id across systems.
        /// </summary>
        public Guid ExternalId { get; private set; } = Guid.NewGuid();
        /// <summary>
        ///     Title name of the location to identify.
        /// </summary>
        public string Name { get; protected set; } = string.Empty;
        /// <summary>
        ///     Short return Description/Instruction to the person responsible
        /// </summary>
        public string ReturnDescription { get; protected set; } = string.Empty;
        /// <summary>
        ///     Office Location's Id that has been selected as ReturnLocation
        /// </summary>
        public Guid LocationId { get; protected set; }

        /// <summary>
        /// Added to prevent Entity framework No suitable constructor found exception.
        /// </summary>
        protected ReturnLocation()
        { }

        public ReturnLocation(string name, string returnDescription, Guid locationId)
        {
            Name = name;
            ReturnDescription = returnDescription;
            LocationId = locationId;
        }

        public void Update(ReturnLocation returnLocationData)
        {
            if (!string.IsNullOrEmpty(returnLocationData.Name))
                Name = returnLocationData.Name;
            if (!string.IsNullOrEmpty(returnLocationData.ReturnDescription))
                ReturnDescription = returnLocationData.ReturnDescription;
            if (returnLocationData.LocationId != LocationId && returnLocationData.LocationId != Guid.Empty)
                LocationId = returnLocationData.LocationId;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return ExternalId;
            yield return Name;
            yield return ReturnDescription;
            yield return LocationId;
        }
    }
}
