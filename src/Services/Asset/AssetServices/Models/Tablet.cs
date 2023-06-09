﻿using AssetServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public class Tablet : HardwareAsset
    {
        [JsonConstructor]
        public Tablet() { }

        public Tablet(Guid externalId, Guid callerId, string serialNumber, string brand, string productName,
            IList<AssetImei> imei, string macAddress)
        {
            ExternalId = externalId;
            SerialNumber = serialNumber;
            Brand = brand;
            ProductName = ProductName = productName[..Math.Min(productName.Length, 50)]; ;
            _imeis.AddRange(imei.ToList());
            MacAddress = macAddress;
            CreatedBy = callerId;
            UpdatedBy = callerId;

            // Store tablet created event with its serial number if applicable
            string text = "id: " + ExternalId.ToString();
            if (!string.IsNullOrEmpty(SerialNumber))
            {
                text = "serial number: " + SerialNumber;
            }
            AddDomainEvent(new AssetCreatedDomainEvent<Tablet>(this, callerId, text));
        }

        public override void UpdateBrand(string brand, Guid callerId)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<Tablet>(this, callerId, previousBrand));
            base.UpdateBrand(brand, callerId);
        }

        public override void UpdateProductName(string model, Guid callerId)
        {
            var previousModel = ProductName;
            ProductName = model[..Math.Min(model.Length, 50)];
            AddDomainEvent(new ModelChangedDomainEvent<Tablet>(this, callerId, previousModel));
            base.UpdateProductName(model, callerId);    
        }

        public override void ChangeSerialNumber(string serialNumber, Guid callerId)
        {
            var previousSerialNumber = SerialNumber;
            SerialNumber = serialNumber;
            AddDomainEvent(new SerialNumberChangedDomainEvent<Tablet>(this, callerId, previousSerialNumber));
            base.ChangeSerialNumber(serialNumber, callerId);
        }
        public override void SetImei(IList<long> imeiList, Guid callerId)
        {
            AddDomainEvent(new IMEIChangedDomainEvent<Tablet>(this, imeiList));
            base.SetImei(imeiList, callerId);
        }
        public override void SetMacAddress(string macAddress, Guid callerId)
        {
            base.SetMacAddress(macAddress, callerId);
            AddDomainEvent(new MacAddressChangedDomainEvent<Tablet>(this, macAddress, callerId));
        }
    }
}
