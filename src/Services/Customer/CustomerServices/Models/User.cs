﻿using System;
using System.Text.Json.Serialization;
using Common.Seedwork;
using CustomerServices.DomainEvents;

namespace CustomerServices.Models
{
    public class User : Entity, IAggregateRoot
    {
        public User(Customer customer, Guid userId, string firstName, string lastName, string email, string mobileNumber, string employeeId)
        {
            Customer = customer;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNumber = mobileNumber;
            EmployeeId = employeeId;
            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        protected User() { }

        public Guid UserId { get; set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Email { get; protected set; }
        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string MobileNumber { get; protected set; }
        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string EmployeeId { get; protected set; }

        public UserPreference UserPreference { get; protected set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

        internal void SetDeleteStatus(bool isDeleted)
        {
            AddDomainEvent(new UserDeletedDomainEvent(this));
            IsDeleted = isDeleted;
        }

        internal void ChangeFirstName(string firstName)
        {
            var oldNameValue = FirstName;
            FirstName = firstName;
            // TODO add domain event
        }

        internal void ChangeLastName(string lastName)
        {
            var oldNameValue = LastName;
            LastName = lastName;
            // TODO add domain event
        }

        internal void ChangeEmailAddress(string email)
        {
            var oldEmailAddress = Email;
            Email = email;
            // TODO add domain event
        }

        internal void ChangeEmployeeId(string employeeId)
        {
            var oldEmployeeId = EmployeeId;
            EmployeeId = employeeId;
            // TODO add domain event
        }
    }
}