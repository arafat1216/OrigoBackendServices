using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public class CustomerLabel : Entity
    {
        /// <summary>
        /// The external Id of this Label
        /// </summary>
        public Guid ExternalId { get; protected set; }

        /// <summary>
        /// The customer this Label belongs to
        /// </summary>
        public Guid CustomerId { get; protected set; }

        /// <summary>
        /// The Label data
        /// <see cref="Label"/>
        /// </summary>
        public Label Label { get; protected set; }

        // Set to protected as DDD best practice
        protected CustomerLabel()
        { }

        /// <summary>
        /// Assign the given label to the given customer
        /// </summary>
        public CustomerLabel(Guid customerId, Label label)
        {
            ExternalId = Guid.NewGuid();
            CustomerId = customerId;
            Label = label;
        }

        public CustomerLabel(Guid externalId, Guid customerId, Label label)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            Label = label;
        }

        /// <summary>
        /// Update the label attribute
        /// </summary>
        /// <param name="label"></param>
        public void PatchLabel(Label label)
        {
            Label = label;
        }
    }
}
