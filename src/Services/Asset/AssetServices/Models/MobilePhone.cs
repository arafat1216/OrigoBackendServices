﻿using AssetServices.DomainEvents;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssetServices.Models
{
    public class MobilePhone : HardwareAsset
    {
        
        public MobilePhone() { }
        
        public MobilePhone(Guid externalId, Guid customerId, Guid callerId, string alias, AssetCategory assetCategory, string serialNumber, string brand, string productName,
            LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<AssetImei> imei, string macAddress, AssetStatus status, string note, string assetTag,
            string description, Guid? managedByDepartmentId = null)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            SerialNumber = serialNumber;
            Brand = brand;
            ProductName = productName;
            LifecycleType = lifecycleType;
            PurchaseDate = purchaseDate;
            AssetHolderId = assetHolderId;
            Imeis = new ReadOnlyCollection<AssetImei>(imei ?? new List<AssetImei>());
            MacAddress = macAddress;
            Status = status;
            Note = note;
            AssetTag = assetTag;
            Description = description;
            AssetCategory = assetCategory;
            ManagedByDepartmentId = managedByDepartmentId;
            Alias = alias;
            CreatedBy = callerId;
            UpdatedBy = callerId;

            // Store MobilePhone with its imei, if applicable
            string text = "id: " + ExternalId.ToString();
            if (Imeis.Count > 0)
            {
                text = "imei: " + Imeis.ElementAt(0).Imei.ToString();
            }

            AddDomainEvent(new AssetCreatedDomainEvent<MobilePhone>(this, callerId, text));
        }

        /// <summary>
        /// Sets the alias of the asset
        /// </summary>
        /// <param name="alias"></param>
        public override void SetAlias(string alias, Guid callerId)
        {
            var previousAlias = Alias;
            Alias = alias;
            AddDomainEvent(new SetAliasDomainEvent<MobilePhone>(this, callerId, previousAlias));
            base.SetAlias(alias, callerId);
        }

        public override void SetLifeCycleType(LifecycleType newLifecycleType, Guid callerId)
        {
            var previousLifecycleType = LifecycleType;
            LifecycleType = newLifecycleType;
            AddDomainEvent(new SetLifeCycleTypeDomainEvent<MobilePhone>(this, callerId, previousLifecycleType));
            base.SetLifeCycleType(newLifecycleType, callerId);
        }

        public override void UpdateAssetStatus(AssetStatus status, Guid callerId)
        {
            var previousStatus = Status;
            Status = status;
            AddDomainEvent(new UpdateAssetStatusDomainEvent<MobilePhone>(this, callerId, previousStatus));
            base.UpdateAssetStatus(status, callerId);
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

        public override void ChangePurchaseDate(DateTime purchaseDate, Guid callerId)
        {
            var previousPurchaseDate = PurchaseDate;
            PurchaseDate = purchaseDate;
            AddDomainEvent(new PurchaseDateChangedDomainEvent<MobilePhone>(this, callerId, previousPurchaseDate));
            base.ChangePurchaseDate(purchaseDate, callerId);
        }

        public override void AssignAssetToUser(Guid? userId, Guid callerId)
        {
            var oldUserId = AssetHolderId;
            AssetHolderId = userId;
            AddDomainEvent(new AssignAssetToUserDomainEvent<MobilePhone>(this, callerId, oldUserId));
            base.AssignAssetToUser(userId, callerId);
        }

        public override void UpdateNote(string note, Guid callerId)
        {
            var previousNote = Note;
            Note = note;
            UpdatedBy = callerId;
            AddDomainEvent(new NoteChangedDomainEvent<MobilePhone>(this, callerId, previousNote));
            base.UpdateNote(note, callerId);
        }

        public override void UpdateDescription(string description, Guid callerId)
        {
            var previousDescription = Description;
            Description = description;
            AddDomainEvent(new DescriptionChangedDomainEvent<MobilePhone>(this, callerId, previousDescription));
            base.UpdateDescription(description, callerId);
        }

        public override void UpdateTag(string tag, Guid callerId)
        {
            var previousTag = AssetTag;
            AssetTag = tag;
            AddDomainEvent(new TagUpdatedDomainEvent<MobilePhone>(this, callerId, previousTag));
            base.UpdateTag(tag, callerId);
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
    }
}
