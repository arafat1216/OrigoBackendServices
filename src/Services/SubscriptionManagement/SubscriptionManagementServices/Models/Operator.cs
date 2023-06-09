﻿using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class Operator : Entity
    {
        public Operator()
        {

        }
        public Operator(int id, string operatorName,string country)
        {
            Id = id;
            OperatorName = operatorName;
            Country = country;
        }
        public Operator(string operatorName, string country, Guid callerId)
        {
            OperatorName = operatorName;
            Country = country;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }
        public Operator(string operatorName, string country, IList<SubscriptionProduct>? subscriptionProducts, IList<CustomerOperatorAccount>? operatorAccounts)
        {
            OperatorName = operatorName;
            Country = country;
        }
        [MaxLength(50)]
        public string OperatorName { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
        public virtual ICollection<SubscriptionProduct>? SubscriptionProducts { get; set; }
        public virtual ICollection<CustomerOperatorAccount>? CustomerOperatorAccounts { get; set; }
        public virtual ICollection<CustomerOperatorSettings>? CustomerOperatorSettings { get; set; }
        public virtual ICollection<CustomerSubscriptionProduct>? CustomerSubscriptionProducts { get; set; }

    }
}
