namespace OrigoApiGateway.Services
{
    public class TechstepCoreWebhookConfiguration : IBaseGatewayOptions
    {
        public string ApiPath { get; set; }
        public string ApiKey { get; set; }
        public string UpdatePath { get; set; }

    }
}
