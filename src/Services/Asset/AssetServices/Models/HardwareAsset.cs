using AssetServices.Exceptions;
using AssetServices.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
                    throw new InvalidAssetImeiException($"{imei}", Guid.Parse("4d88ed3b-c5f1-498d-ac85-82ed92c23884"));
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
                    throw new InvalidAssetImeiException($"{imei}", Guid.Parse("4d88ed3b-c5f1-498d-ac85-82ed92c23884"));
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
        public virtual void SetMacAddress(string macAddress, Guid callerId)
        {
            if (ValidateMacAddress(macAddress))
            {
                MacAddress = macAddress;
                UpdatedBy = callerId;
                LastUpdatedDate = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidAssetMacAddressException($"{macAddress}",Guid.Parse("509ef045-4b9a-4587-97c8-8f473460b7bf"));
            }
        }
        protected bool ValidateMacAddress(string? macAddress)
        {
            if (macAddress == null || string.IsNullOrEmpty(macAddress)) throw new InvalidAssetMacAddressException($"EMPTY", Guid.Parse("4dfa8a73-bd31-4eb8-96bf-df2ecbfe5471"));
            

            var regex = "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})|([0-9a-fA-F]{4}\\.[0-9a-fA-F]{4}\\.[0-9a-fA-F]{4})$";

            return Regex.IsMatch(macAddress, regex);
           
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
