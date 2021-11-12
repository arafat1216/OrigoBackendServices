using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class OrigoUpdateAsset
    {
        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The date the asset was purchased.
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// A note containing additional information or comments for the asset.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// A description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tags associated with this asset.
        /// </summary>
        public string AssetTag { get; set; }

        /// <summary>
        /// The imei value of the asset
        /// </summary>
        public IList<long> Imei { get; set; }

        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialNumber { get; set; }
    }
}
