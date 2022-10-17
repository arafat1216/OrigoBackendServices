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

        public UserPreference(string language, Guid callerId, bool? isAssetTileClosed = null, bool? isSubscriptionTileClosed = null)
        {
            Language = language;
            IsAssetTileClosed = isAssetTileClosed;
            IsSubscriptionTileClosed = isSubscriptionTileClosed;
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
    }
}