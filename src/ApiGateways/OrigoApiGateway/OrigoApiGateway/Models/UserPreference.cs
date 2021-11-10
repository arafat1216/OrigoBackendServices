using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class UserPreference
    {
        public UserPreference() { }
        public UserPreference(UserPreferenceDTO userPreference)
        {
            Language = (userPreference == null) ? "" : userPreference.Language;
        }
        public string Language { get; set; }
    }
}
