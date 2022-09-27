namespace OrigoApiGateway.Services
{
    public class CustomerConfiguration : IBaseGatewayOptions
    {
        public string ApiPath { get; set; }
        public Guid TechstepPartnerId { get; set; }

    }
}