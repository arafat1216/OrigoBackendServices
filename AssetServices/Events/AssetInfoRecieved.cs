using System;
using Dolittle.SDK.Events;

namespace AssetServices.Events
{
    [EventType("f3ffc6a9-bb62-44c8-8feb-08bfcf258cba")]
    public class AssetInfoReceived : SystemEvent
    {
        public AssetInfoReceived(DateTime deliveryDate, string organizationNr, string techstepCustomerId, string techstepOrderNr)
        {
            DeliveryDate = deliveryDate;
            OrganizationNr = organizationNr;
            TechstepCustomerId = techstepCustomerId;
            TechstepOrderNr = techstepOrderNr;
        }

        /// <summary>
        /// The date the customer receives the product, or the closest available date.
        /// This is used to determine warranty, device return date, etc.
        /// </summary>
        /// <value></value>
        public DateTime DeliveryDate { get; }

        /// <summary>
        /// The order-id / order-number that is used by Techstep for a specific order.
        /// </summary>
        /// <value></value>
        public string TechstepOrderNr { get; set; }

        /// <summary>
        /// The customers organization number.
        /// </summary>
        /// <value></value>
        public string OrganizationNr { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string TechstepCustomerId { get; set; }
    }
}