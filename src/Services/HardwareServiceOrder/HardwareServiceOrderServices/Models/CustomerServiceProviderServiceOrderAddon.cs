using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     The join-table entity between <see cref="CustomerServiceProvider"/> and <see cref="ServiceOrderAddon"/>.
    /// </summary>
    public class CustomerServiceProviderServiceOrderAddon : EntityV2, IDbSetEntity
    {
        public int CustomerServiceProviderId { get; set; }
        public int ServiceOrderAddonId { get; set; }


        public virtual CustomerServiceProvider? CustomerServiceProvider { get; set; }
        public virtual ServiceOrderAddon? ServiceOrderAddon { get; set; }
    }
}
