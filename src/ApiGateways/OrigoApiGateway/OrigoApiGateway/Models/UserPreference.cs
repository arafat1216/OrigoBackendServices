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
        public string Language { get; set; }
    }
}
