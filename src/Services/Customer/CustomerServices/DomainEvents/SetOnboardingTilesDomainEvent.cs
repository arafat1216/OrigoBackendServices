using Common.Logging;
using CustomerServices.Models;
using System;

namespace CustomerServices.DomainEvents
{
    class SetOnboardingTilesDomainEvent : BaseEvent
    {
        public SetOnboardingTilesDomainEvent(UserPreference userPreference, User user, Guid callerId) : base(user.UserId)
        {
            User = user;
            CallerId = callerId;
            UserPreference = userPreference;
        }

        public User User { get; protected set; }
        public UserPreference UserPreference { get; protected set; }
        public Guid CallerId { get; protected set; }

        public override string EventMessage()
        {
            return $"Onboarding Tiles Changed for User {User.Id}; AssetTile: '{UserPreference.IsAssetTileClosed}', SubscriptionTile: '{UserPreference.IsAssetTileClosed}' by {CallerId}";
        }
    }
}
