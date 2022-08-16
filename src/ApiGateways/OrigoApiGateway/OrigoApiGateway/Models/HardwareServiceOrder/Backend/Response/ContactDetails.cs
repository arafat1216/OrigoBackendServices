#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response
{
    public class ContactDetails
    {
        /// <summary>
        ///     A TEMPORARY alias for the <see cref="UserId"/> property. This is a temporarily alias to prevent breaking changes in the frontend following
        ///     the renaming, but it will soon be removed!
        /// </summary>
        [Obsolete("This value has been renamed to 'UserId'. This is a temporarily alias to prevent breaking changes, but it will soon be removed!")]
        public Guid Id
        {
            get { return UserId; }
            set { UserId = value; }
        }

        /// <summary>
        ///     The persons user-ID.
        /// </summary>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        ///     The persons first name.
        /// </summary>
        /// <example> John </example>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        ///     The persons last name.
        /// </summary>
        /// <example> Doe </example>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        ///     The persons email.
        /// </summary>
        /// <example> demo@user.com </example>
        [Required]
        [MaxLength(320)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     The persons phone-number in <c>E.164</c> format.
        /// </summary>
        /// <example> +4790090900 </example>
        [Phone]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        [StringLength(maximumLength: 15)] // The longest possible length for a valid E.164 phone-number
        public string? PhoneNumber { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDetails"/> intended for use with JSON (de-)serializers, AutoMapper, unit-testing
        ///     and other automated tools.
        /// </summary>
        [Obsolete("Reserved for use by JSON (de-)serializers, AutoMapper, unit-testing and other automated tools.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ContactDetails()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDetails"/>-class.
        /// </summary>
        /// <param name="userId"> The persons ID. </param>
        /// <param name="firstName"> The persons first-name. </param>
        /// <param name="lastName"> The persons last-name. </param>
        /// <param name="email"> The persons e-mail. </param>
        /// <param name="phoneNumber"> The persons phone-number in E.164 format. </param>
        public ContactDetails(Guid userId, string firstName, string lastName, string email, string? phoneNumber)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
