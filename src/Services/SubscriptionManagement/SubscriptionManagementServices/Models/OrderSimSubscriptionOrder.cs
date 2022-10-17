using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using SubscriptionManagementServices.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionManagementServices.Models
{
    public class OrderSimSubscriptionOrder : Entity, ISubscriptionOrder
    {

        public OrderSimSubscriptionOrder()
        {
        }

        public OrderSimSubscriptionOrder(string sendToName, string street, string postcode, string city, string country, string operatorName, int quantity, Guid organizationId, Guid callerId)
        {
            SendToName = sendToName;
            Street = street;
            Postcode = postcode;
            City = city;
            Country = country;
            OperatorName = operatorName;
            Quantity = quantity;
            OrganizationId = organizationId;
            SubscriptionOrderId = Guid.NewGuid();
            CreatedBy = callerId;
            AddDomainEvent(new OrderSimSubscriptionOrderCreatedDomainEvent(this, callerId));
        }

        /// <summary>
        /// The recipient name of the sim card.
        /// </summary>
        public string SendToName { get; protected set; }
        /// <summary>
        /// Send to either private or business address
        /// </summary>
        public string Street { get; protected set; }

        public string Postcode { get; protected set; }

        public string City { get; protected set; }
        [MaxLength(2)]

        public string Country { get; protected set; }

        public string OperatorName { get; protected set; }
        public int Quantity { get; protected set; }
        public Guid OrganizationId { get; protected set; }

        #region ISubscriptionOrder Implementation

        [NotMapped] public OrderTypes OrderType => OrderTypes.OrderSim;

        [NotMapped] public string PhoneNumber => string.Empty;

        [NotMapped] public string NewSubscriptionOrderOwnerName => SendToName;

        [NotMapped] public DateTime OrderExecutionDate => CreatedDate;

        public Guid SubscriptionOrderId { get; set; }
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}
