
namespace Common.Configuration
{
    /// <summary>
    /// Techstep partner common attributes
    /// </summary>
    public record TechstepPartnerConfiguration
    {
        /// <summary>
        /// Techstep's partner id
        /// </summary>
        public Guid PartnerId { get; set; }

    }
}
