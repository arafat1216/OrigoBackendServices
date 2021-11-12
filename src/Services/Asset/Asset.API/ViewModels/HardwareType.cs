using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public record HardwareType : Asset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; protected set; }

        /// <summary>
        /// The imei of the asset. Applicable to assets with category Mobile phone.
        /// </summary>
        public IList<long> Imei { get; protected set; }

        /// <summary>
        /// The mac address of the asset
        /// </summary>
        public string MacAddress { get; protected set; }
    }
}
