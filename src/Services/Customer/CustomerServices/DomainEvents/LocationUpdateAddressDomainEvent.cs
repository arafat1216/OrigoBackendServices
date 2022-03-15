using Common.Logging;
using CustomerServices.Models;

namespace CustomerServices.DomainEvents
{
    class LocationUpdateAddressDomainEvent : BaseEvent
    {
        public LocationUpdateAddressDomainEvent(Location location, int number, string oldAddress) : base(location.LocationId)
        {
            Location = location;
            Number = number;
            OldAddress = oldAddress;
        }

        public Location Location { get; protected set; }
        public string OldAddress { get; protected set; }
        public int Number { get; protected set; }

        public override string EventMessage()
        {
            if (Number == 1)
            {
                return $"Location address1 changed from {OldAddress} to {Location.Address1}.";
            }
            if (Number == 2)
            {
                return $"Location address2 changed from {OldAddress} to {Location.Address2}.";
            }
            return string.Empty;


        }
    }
}
