using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    // TODO: Implement this once we are introducing aftermarket services
    internal class PartnerSettings: EntityV2, IAggregateRoot
    {
        /*
         * IDs:
         * 
         * PartnerId + ServiceProviderId (composite key?)
         * 
         */

        /*
         * Data:
         * 
         * Partner spesific Conmodo IDs
         *   - Account IDs for recycle
         *   - Account IDs for return
         */

    }
}
