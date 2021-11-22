﻿using AssetServices.DomainEvents;
using AssetServices.Exceptions;
using AssetServices.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AssetServices.Models
{
    public abstract class HardwareAsset : Asset
    {
        /// <summary>
        /// The unique serial number for the asset. For mobile phones and other devices
        /// where an IMEI number also exists, the IMEI will be used here.
        /// </summary>
        [Required]
        public string SerialNumber { get; set; }

        /// <summary>
        /// A list of all the IMEI numbers this asset has
        /// </summary>
        public IList<AssetImei> Imeis { get; set; }

        /// <summary>
        /// The mac-address of the asset
        /// </summary>
        public string MacAddress { get; protected set; }

        public void ChangeSerialNumber(string serialNumber)
        {
            var previousSerialNumber = SerialNumber;
            SerialNumber = serialNumber;
            AddDomainEvent(new SerialNumberChangedDomainEvent(this, previousSerialNumber));
        }

        /// <summary>
        /// Set the imei for the asset.
        /// Erases existing imeis on asset.
        /// 
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imeiList"></param>
        public void SetImei(IList<long> imeiList)
        {
            foreach (long imei in imeiList)
            {
                if (!AssetValidatorUtility.ValidateImei(imei.ToString()))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {imei}");
                }
            }
            Imeis?.Clear();
            Imeis = imeiList.Select(i => new AssetImei(i)).ToList();
        }

        /// <summary>
        /// Appends an Imei for the device.
        /// Imei is a comma separated string, and can hold multiple imei values
        /// </summary>
        /// <param name="imeiList"></param>
        public void AddImei(IList<long> imeiList)
        {
            foreach (long imei in imeiList)
            {
                if (!AssetValidatorUtility.ValidateImei(imei.ToString()))
                {
                    throw new InvalidAssetDataException($"Invalid imei: {imei}");
                }

                if (!Imeis.Any(i => i.Imei == imei))
                {
                    Imeis.Add(new AssetImei(imei));
                }
            }
        }

        /// <summary>
        /// Sets the macaddress of the asset
        /// </summary>
        /// <param name="macAddress"></param>
        public void SetMacAddress(string macAddress)
        {
            MacAddress = macAddress;
        }
    }
}
