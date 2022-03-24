using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public class UserPreference
    {
        /// <summary>
        ///     The user's language preference, using the <c>ISO 639-1</c> standard.
        /// </summary>
        [RegularExpression("^[a-z]{2}")] // Exactly 2 lowercase characters
        public string Language { get; set; }
    }
}