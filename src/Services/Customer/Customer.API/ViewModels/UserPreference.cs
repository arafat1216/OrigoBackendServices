namespace Customer.API.ViewModels
{
    public class UserPreference
    {
        public UserPreference() { }
        public UserPreference(CustomerServices.Models.UserPreference userPreference)
        {
            Language = userPreference.Language;
        }

        public string Language { get; set; }
    }
}