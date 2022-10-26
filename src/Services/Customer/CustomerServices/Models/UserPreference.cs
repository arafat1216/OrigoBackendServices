using Common.Enums;
using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerServices.Models
{
    public class UserPreference : Entity
    {
        protected UserPreference() { }

        public UserPreference(string language, Guid callerId, bool? isAssetTileClosed = null, bool? isSubscriptionTileClosed = null,bool ? subscriptionIsHandledForOffboarding = null)
        {
            Language = language;
            IsAssetTileClosed = isAssetTileClosed;
            IsSubscriptionTileClosed = isSubscriptionTileClosed;
            SubscriptionIsHandledForOffboarding = subscriptionIsHandledForOffboarding;
            CreatedBy = callerId;
        }

        [StringLength(2, ErrorMessage = "Country code max length is 2")]
        public string Language { get; set; }
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsAssetTileClosed { get; private set; } = true;
        /// <summary>
        /// Is onboarding Asset Tile Closed by the User.
        /// </summary>
        public bool? IsSubscriptionTileClosed { get; private set; } = true;
        /// <summary>
        /// Has the user dealt with the subscription they have related to the offboarding task.
        /// </summary>
        public bool? SubscriptionIsHandledForOffboarding { get; private set; } = false;

        internal void SetDeleteStatus(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }

        public void SetOnboardingTiles(User user, bool? isAssetTileClosed, bool? isSubscriptionTileClosed, Guid callerId)
        {
            UpdatedBy = callerId;
            IsAssetTileClosed = isAssetTileClosed;
            IsSubscriptionTileClosed = isSubscriptionTileClosed;
            AddDomainEvent(new SetOnboardingTilesDomainEvent(this, user, callerId));
            LastUpdatedDate = DateTime.UtcNow;
        }
        /// <summary>
        /// Handles if the subscription tile for offboarding should be shown or not. 
        /// If the bool is true then user has handled the subscription connected to the customer.
        /// </summary>
        /// <param name="callerId">The identity of the person making the call.</param>
        internal void HandleSubscriptionForOffboarding(bool? subscriptionIsHandledForOffboarding, Guid callerId)
        {
            UpdatedBy = callerId;
            SubscriptionIsHandledForOffboarding = subscriptionIsHandledForOffboarding;
            AddDomainEvent(new OffboardingSubscriptionIsHandledDomainEvent(this, callerId));
            LastUpdatedDate = DateTime.UtcNow;
        }
    }
}