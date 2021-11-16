using System.ComponentModel.DataAnnotations;

namespace AssetServices.Models
{
    public abstract class SoftwareAsset : Asset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        [Required]
        public string SerialKey { get; set; }
    }
}
