namespace OrigoApiGateway.Services
{
    public interface IBaseGatewayOptions
    {
        string ApiBaseUrl { get; set; }
        string ApiPort { get; set; }
        string ApiPath { get; set; }
    }
}
