using AssetServices.Exceptions;
using AssetServices.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public abstract class HardwareAsset : Asset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        [JsonInclude]
        public string? SerialNumber { get; protected set; }

        /// <summary>
        /// A list of all the IMEI numbers this asset has
        /// </summary>
        protected readonly List<AssetImei> _imeis = new();
        public IReadOnlyCollection<AssetImei> Imeis => _imeis.AsReadOnly();

        /// <summary>
        /// The mac-address of the asset
        /// </summary>
        [JsonInclude]
        public string? MacAddress { get; protected set; }

        public virtual void ChangeSerialNumber(string serialNumber, Guid callerId)
        {
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;

        }

        /// <summary>
        /// Set the imei for the asset.
        /// Erases existing imeis on asset.
        /// 
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imeiList"></param>
        public virtual void SetImei(IList<long> imeiList, Guid callerId)
        {
            foreach (long imei in imeiList)
            {
                if (!AssetValidatorUtility.ValidateImei(imei.ToString()))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {imei}");
                }
            }
            _imeis.Clear();
            _imeis.AddRange(imeiList.Select(i => new AssetImei(i)).ToList());
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Appends an Imei for the device.
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imeiList"></param>
        public void AddImei(IList<long> imeiList)
        {
            List<AssetImei> imeis = new List<AssetImei>();
            foreach (long imei in imeiList)
            {
                if (!AssetValidatorUtility.ValidateImei(imei.ToString()))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {imei}");
                }

                if (Imeis.All(i => i.Imei != imei))
                {
                    imeis.Add(new AssetImei(imei));
                }
            }
            _imeis.AddRange(imeis);
        }

        /// <summary>
        /// Sets the macaddress of the asset
        /// </summary>
        /// <param name="macAddress"></param>
        public void SetMacAddress(string macAddress)
        {
            MacAddress = macAddress;
        }

        protected override bool ValidateAsset()
        {
            ErrorMsgList = new List<string>();

            bool validAsset = true;
            // General (all types)
            if (string.IsNullOrEmpty(Brand))
            {
                ErrorMsgList.Add("Brand - Cannot be null or empty");
                validAsset = false;
            }

            if (string.IsNullOrEmpty(ProductName))
            {
                ErrorMsgList.Add("Model - Cannot be null or empty");
                validAsset = false;
            }

            if (!Imeis.Any() && string.IsNullOrWhiteSpace(SerialNumber))
            {
                ErrorMsgList.Add("Imeis or SerialNumber - An asset must have at least one identifying attribute");
                validAsset = false;
            }

            return validAsset;
        }
    }
}
