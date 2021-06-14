using System;

namespace OrigoApiGateway.Models
{
    public class OrigoUpdateAsset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

    }
}
