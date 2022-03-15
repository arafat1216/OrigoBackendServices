namespace SubscriptionManagementServices
{
    public record EmailConfiguration
    {
        public string BaseUrl { get; set; }
        public string Partner { get; set; }
        public string Language { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public List<string> Recipient { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public Dictionary<string, string> Templates { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string TemplateContainer { get; set; }
    }
}
