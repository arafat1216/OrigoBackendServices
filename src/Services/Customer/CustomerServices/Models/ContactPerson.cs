using System.Collections.Generic;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class ContactPerson : ValueObject
    {
        public ContactPerson(string fullName, string email, string phoneNumber)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string FullName { get; }

        public string Email { get; }

        public string PhoneNumber { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return FullName;
            yield return Email;
            yield return PhoneNumber;
        }

    }
}
