using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace CustomerServices.Models
{
    [Owned]
    public class ContactPerson : ValueObject
    {
        /// <summary>
        /// Needed to avoid 'Not suitable constructor found' error.
        /// </summary>
        public ContactPerson() { }

        public ContactPerson(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = (firstName == null) ? "" : firstName.Trim();
            LastName = (lastName == null) ? "" : lastName.Trim();
            Email = (email == null) ? "" : email.Trim();
            PhoneNumber = (phoneNumber == null) ? "" : phoneNumber.Trim();
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        [MaxLength(320)]
        public string Email { get; private set; }
        [MaxLength(15)]
        public string PhoneNumber { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return FirstName;
            yield return LastName;
            yield return Email;
            yield return PhoneNumber;
        }

    }
}
