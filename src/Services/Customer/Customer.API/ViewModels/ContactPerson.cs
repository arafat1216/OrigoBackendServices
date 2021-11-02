﻿namespace Customer.API.ViewModels
{
    public record ContactPerson
    {
        public ContactPerson(CustomerServices.Models.ContactPerson customerContactPerson)
        {
            FirstName = (customerContactPerson == null) ? "" : customerContactPerson.GetFirstName();
            LastName = (customerContactPerson == null) ? "" : customerContactPerson.GetLastName();
            Email = (customerContactPerson == null) ? "" : customerContactPerson.Email;
            PhoneNumber = (customerContactPerson == null) ? "" : customerContactPerson.PhoneNumber;
        }

        public ContactPerson(){}

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}