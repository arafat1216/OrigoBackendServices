using System;
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

        protected User(){}

        public Guid UserId { get; set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Email { get; protected set; }
        public string MobileNumber { get; protected set; }
        public string EmployeeId { get; protected set; }
        public Customer Customer { get; set; }
    }
}