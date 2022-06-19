namespace HardwareServiceOrderServices.Configuration
{
    public record ServiceProviderConfiguration
    {
        public Dictionary<string, ProviderConfiguration> Providers { get; set; }
    }


    public record ProviderConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ApiUsername { get; set; }
        public string ApiPassword { get; set; }
    }
}
