namespace OrigoApiGateway.Services
{
    public class AssetConfiguration : IBaseGatewayOptions
    {
        public string ApiBaseUrl { get; set; }
        public string ApiPort { get; set; }
        public string ApiPath { get; set; }
    }
}