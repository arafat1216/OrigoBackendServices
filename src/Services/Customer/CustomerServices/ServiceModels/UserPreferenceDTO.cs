using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record UserPreferenceDTO
    {
        public string Language { get; init; }
    }
}