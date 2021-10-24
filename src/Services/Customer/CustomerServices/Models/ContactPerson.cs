using System.Collections.Generic;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace CustomerServices.Models
{
    [Owned]
    public class ContactPerson : ValueObject
    {
        public ContactPerson(string fullName, string email, string phoneNumber)
        {
            FullName = (fullName == null) ? "" : fullName;
            Email = (email == null) ? "" : email;
            PhoneNumber = (phoneNumber == null) ? "" : phoneNumber;
        }

        public string FullName { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return FullName;
            yield return Email;
            yield return PhoneNumber;
        }

    }
}
