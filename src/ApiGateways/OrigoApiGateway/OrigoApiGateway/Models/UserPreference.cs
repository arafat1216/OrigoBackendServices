using System.Reflection.Metadata;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class UserPreference
    {
        public UserPreference()
        {
            Language = string.Empty;
        }
        public string Language { get; set; }
    }
}
