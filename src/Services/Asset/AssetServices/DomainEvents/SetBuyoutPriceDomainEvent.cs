using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class SetBuyoutPriceDomainEvent: BaseEvent
    {
        public SetBuyoutPriceDomainEvent(CategoryLifeCycleSetting categorylifeCycleSetting, Guid customerId, Guid callerId) : base()
        {
            CategoryLifeCycleSetting = categorylifeCycleSetting;
            CallerId = callerId;
            CustomerId = customerId;
        }

        public CategoryLifeCycleSetting CategoryLifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid CustomerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Min Buyout Price has been set for Customer: {CustomerId}; AssetCategory: {CategoryLifeCycleSetting.AssetCategoryName}; Amount: {CategoryLifeCycleSetting.MinBuyoutPrice}";
        }
    }
}
