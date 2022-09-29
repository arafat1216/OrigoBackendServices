namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class UserPreference
    {
        public UserPreference()
        {
            Language = string.Empty;
        }
        
        [RegularExpression(@"^[a-z]{2}", ErrorMessage = "Language code needs to be exactly 2 lowercase characters.")] // Exactly 2 lowercase characters
        public string Language { get; set; }
    }
}
