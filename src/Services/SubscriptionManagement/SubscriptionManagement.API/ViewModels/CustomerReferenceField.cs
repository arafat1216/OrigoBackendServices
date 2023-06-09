﻿namespace SubscriptionManagement.API.ViewModels
{
    public record CustomerReferenceField
    {
        public CustomerReferenceField(string name, string type, int id)
        {
            Name = name;
            Type = type;
            Id = id;
        }

        public int Id { get; set; }
        /// <summary>
        /// The name of the reference field. Set by the customer to tell what kind of information is put in this
        /// reference field
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of customer reference field. Which can either by:
        /// User : Information related to the employee.
        /// Account: Information related to the customer.
        /// </summary>
        public string Type { get; set; }
    }
}
