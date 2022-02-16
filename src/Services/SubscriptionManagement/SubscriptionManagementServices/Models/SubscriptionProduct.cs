﻿using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionProduct : Entity
    {
        public SubscriptionProduct()
        {

        }
        public SubscriptionProduct(string subscriptionName, Operator @operator, IList<DataPackage>? dataPackages, Guid callerId)
        {

            SubscriptionName = subscriptionName;
            Operator = @operator;
            OperatorId = @operator.Id;
            if (dataPackages != null)
            {
                _dataPackages.AddRange(dataPackages);
            }
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public string SubscriptionName { get; set; }
        public Operator Operator { get; set; }
        public int OperatorId { get; set; }

        private List<DataPackage> _dataPackages = new();
        public ICollection<DataPackage> DataPackages => _dataPackages.AsReadOnly();

        public void SetDataPackages(IList<string> dataPackages, Guid callerId)
        {
            _dataPackages = new List<DataPackage>();
            foreach (var dataPackageName in dataPackages)
            {
                _dataPackages.Add(new DataPackage(dataPackageName, callerId));
            }
        }
    }
}
