
using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class UpdateMinBuyoutPriceDomainEvent: BaseEvent
    {
        public UpdateMinBuyoutPriceDomainEvent(CategoryLifeCycleSetting categorylifeCycleSetting, decimal previousAmount, Guid customerId, Guid callerId) : base()
        {
            CategoryLifeCycleSetting = categorylifeCycleSetting;
            CallerId = callerId;
            CustomerId = customerId;
        }

        public CategoryLifeCycleSetting CategoryLifeCycleSetting { get; protected set; }
        public Guid CallerId { get; protected set; }
        public Guid CustomerId { get; protected set; }
        public decimal PreviousAmount { get; protected set; }

        public override string EventMessage()
        {
            return $"Min Buyout Price has been changed for Customer: {CustomerId}; AssetCategory: {CategoryLifeCycleSetting.AssetCategoryName};From Amount: {PreviousAmount} To Amount: {CategoryLifeCycleSetting.MinBuyoutPrice}";
        }
    }
}
