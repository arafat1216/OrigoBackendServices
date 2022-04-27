using AssetServices.DomainEvents;
using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AssetServices.Models
{
    public class LifeCycleSetting : Entity, IAggregateRoot
    {
        public LifeCycleSetting(Guid customerId, bool buyoutAllowed, Guid callerId)
        {
            CustomerId = customerId;
            BuyoutAllowed = buyoutAllowed;
            CreatedBy = callerId;
        }

        public LifeCycleSetting()
        {

        }

        /// <summary>
        /// The external uniquely identifying id across systems.
        /// </summary>
        public Guid ExternalId { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The customer for this lifecycle Setting.
        /// </summary>
        public Guid CustomerId { get; init; }

        /// <summary>
        /// Is buyout feature on or off for this customer.
        /// </summary>
        public bool BuyoutAllowed { get; protected set; }

        private readonly List<CategoryLifeCycleSetting> _categoryLifeCycleSettings = new();

        /// <summary>
        /// All Category specific settings associated with this lifecycle setting.
        /// </summary>
        public IReadOnlyCollection<CategoryLifeCycleSetting> CategoryLifeCycleSettings => _categoryLifeCycleSettings.AsReadOnly();

        /// <summary>
        /// Update Buyout Allowed property for this setting.
        /// </summary>
        /// <param name="buyoutAllowed">The data for updating buyoutAllowed</param>
        /// <param name="callerId">The userid making this assignment</param>
        public void UpdateBuyoutAllowed(bool buyoutAllowed, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var previousStatus = BuyoutAllowed;
            AddDomainEvent(new UpdateBuyoutAllowedDomainEvent(this, callerId, previousStatus));
            BuyoutAllowed = buyoutAllowed;
        }

        /// <summary>
        /// Set Min Buyout Price for this setting and Category.
        /// </summary>
        /// <param name="buyoutPrice">The amount that will be set min buyout price</param>
        /// <param name="categoryId">The specific Asset Category Id for this setting</param>
        /// <param name="callerId">The userid making this assignment</param>
        public void SetMinBuyoutPrice(decimal buyoutPrice, int categoryId, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            var categorySetting = new CategoryLifeCycleSetting()
            {
                AssetCategoryId = categoryId,
                MinBuyoutPrice = buyoutPrice
            };
            _categoryLifeCycleSettings.Add(categorySetting);
            AddDomainEvent(new SetBuyoutPriceDomainEvent(categorySetting, CustomerId, callerId));
        }

    }

}
