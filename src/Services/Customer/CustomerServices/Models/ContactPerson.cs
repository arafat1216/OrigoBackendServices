using System.Collections.Generic;
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
            firstName = (firstName == null) ? "" : firstName;
            lastName = (lastName == null) ? "" : "," + lastName; // Add a ',' so we can extract firstname vs lastname later
            FullName = firstName + lastName;
            Email = (email == null) ? "" : email;
            PhoneNumber = (phoneNumber == null) ? "" : phoneNumber;
        }

        public string FullName { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public void PatchContactPerson(string fullName, string email, string phoneNumber)
        {
            if (fullName != null)
                FullName = fullName;
            if (email != null)
                Email = email;
            if (phoneNumber != null)
                PhoneNumber = phoneNumber;
        }

        public string GetFirstName()
        {
            var nameList = FullName.Split(',');
            return nameList[0];
        }

        public string GetLastName()
        {
            var nameList = FullName.Split(',');
            var lName = (nameList.Length == 1) ? "" : nameList[1]; // "".Split() == 1, ",xxx".Split() == 2: 0 not possible.
            return lName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return FullName;
            yield return Email;
            yield return PhoneNumber;
        }

    }
}
