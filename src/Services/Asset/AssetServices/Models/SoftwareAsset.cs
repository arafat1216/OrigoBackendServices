using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Models
{
    public abstract class SoftwareAsset : Asset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        public string SerialKey { get; set; }

        protected override bool ValidateAsset()
        {
            ErrorMsgList = new List<string>();

            bool validAsset = true;
            // General (all types)
            if (CustomerId == Guid.Empty)
            {
                ErrorMsgList.Add("CustomerId - Cannot be Guid.Empty");
                validAsset = false;
            }

            if (string.IsNullOrWhiteSpace(Brand))
            {
                ErrorMsgList.Add("Brand - Cannot be null or empty");
                validAsset = false;
            }

            if (string.IsNullOrWhiteSpace(ProductName))
            {
                ErrorMsgList.Add("Model - Cannot be null or empty");
                validAsset = false;
            }

            if (PurchaseDate == DateTime.MinValue)
            {
                ErrorMsgList.Add("PurchaseDate - Cannot be DateTime.MinValue");
                validAsset = false;
            }

            if (PurchaseDate > DateTime.UtcNow)
            {
                ErrorMsgList.Add("PurchaseDate - Cannot be set in the future");
                validAsset = false;
            }

            if (string.IsNullOrWhiteSpace(SerialKey))
            {
                ErrorMsgList.Add("SerialKey - Cannot be null or empty");
                validAsset = false;
            }

            return validAsset;
        }
    }
}
