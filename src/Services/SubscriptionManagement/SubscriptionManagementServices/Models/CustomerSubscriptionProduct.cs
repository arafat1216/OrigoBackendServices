﻿using System.Collections.ObjectModel;
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : Entity
    {
        public CustomerSubscriptionProduct()
        {
        }
        public CustomerSubscriptionProduct(string subscriptionName, Operator @operator, Guid callerId, IList<DataPackage>? dataPackages)
        {
            SubscriptionName = subscriptionName;
            Operator = @operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
        }

        public CustomerSubscriptionProduct(SubscriptionProduct globalSubscriptionProduct, Guid callerId, IList<DataPackage>? dataPackages)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
            SubscriptionName = globalSubscriptionProduct.SubscriptionName;
            Operator = globalSubscriptionProduct.Operator;
            if (dataPackages != null) _dataPackages.AddRange(dataPackages);
        }

        public string SubscriptionName { get; set; }
        public Operator Operator { get; set; }

        private List<DataPackage> _dataPackages = new();
        public ICollection<DataPackage> DataPackages => _dataPackages.AsReadOnly();

        public void SetDataPackages(ICollection<DataPackage>? globalDataPackages,IList<string> selectedDataPackages, Guid callerId)
        {
            _dataPackages.Clear();
            foreach (var select in selectedDataPackages)
            {

                var dataPackage = globalDataPackages.FirstOrDefault(a=>a.DataPackageName == select);

                

                if (dataPackage != null)
                {
                    _dataPackages.Add(dataPackage);
                }
            }
        }



        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }

        public ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}