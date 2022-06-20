namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class DeleteContent
    {
        public Guid CallerId { get; set; }
        public bool hardDelete { get; set; }
    }
}
