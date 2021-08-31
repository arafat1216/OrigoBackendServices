using System.ComponentModel.DataAnnotations;

namespace CustomerServices.Models
{
    public class UserPreference
    {
        public User User { get; protected set; }

        [StringLength(2, ErrorMessage = "Country code max length is 2")]
        public string Language { get; set; }
    }
}