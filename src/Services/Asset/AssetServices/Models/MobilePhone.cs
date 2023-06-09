﻿using AssetServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.Attributes;
using AssetServices.Utility;
using YamlDotNet.Core.Tokens;

namespace AssetServices.Models
{
    public class MobilePhone : HardwareAsset
    {
        
        public MobilePhone() { }
        
        public MobilePhone(Guid externalId, Guid callerId, string serialNumber, string brand, string productName,
            IList<AssetImei> imei, string macAddress)
        {
            ExternalId = externalId;
            SerialNumber = serialNumber;
            Brand = brand;
            ProductName = productName[..Math.Min(productName.Length, 50)];
            _imeis.AddRange(imei);
            MacAddress = macAddress;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public override void UpdateBrand(string brand, Guid callerId)
        {
            var previousBrand = Brand;
            Brand = brand;
            AddDomainEvent(new BrandChangedDomainEvent<MobilePhone>(this, callerId, previousBrand));
            base.UpdateBrand(brand, callerId);
        }

        public override void UpdateProductName(string model, Guid callerId)
        {
            var previousModel = ProductName;
            ProductName = model;
            AddDomainEvent(new ModelChangedDomainEvent<MobilePhone>(this, callerId, previousModel));
            base.UpdateProductName(model, callerId);
        }

        public override void ChangeSerialNumber(string serialNumber, Guid callerId)
        {
            var previousSerialNumber = SerialNumber;
            SerialNumber = serialNumber;
            AddDomainEvent(new SerialNumberChangedDomainEvent<MobilePhone>(this, callerId, previousSerialNumber));
            base.ChangeSerialNumber(serialNumber, callerId);
        }
        public override void SetImei(IList<long> imeiList, Guid callerId)
        {
            AddDomainEvent(new IMEIChangedDomainEvent<MobilePhone>(this, imeiList));
            base.SetImei(imeiList,callerId);
        }
        public override void SetMacAddress(string macAddress, Guid callerId)
        {
            base.SetMacAddress(macAddress, callerId);
            AddDomainEvent(new MacAddressChangedDomainEvent<MobilePhone>(this, macAddress, callerId));

        }

        public override bool ValidateAsset()
        {
            var validAsset = base.ValidateAsset();
            var imeis = string.Join(',', Imeis.Select(i => i.Imei));
            if (AssetValidatorUtility.ValidateImeis(imeis)) return validAsset;
            ErrorMsgList.Add($"Invalid Imei(s) {imeis} for mobile phone");
            validAsset = false;
            return validAsset;
        }
    }
}
