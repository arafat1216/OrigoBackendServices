using OrigoApiGateway.Models.BackendDTO;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public abstract class HardwareSuperType : OrigoAsset
    {
        /// <summary>
        /// The imei of the device. Applicable to devices with category Mobile device.
        /// </summary>
        public IList<long> Imei { get; protected set; }

        /// <summary>
        /// The mac address of the device.
        /// </summary>
        public string MacAddress { get; protected set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; protected set; }
    }
}
