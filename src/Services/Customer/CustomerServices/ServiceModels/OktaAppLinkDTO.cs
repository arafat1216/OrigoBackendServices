namespace CustomerServices.ServiceModels
{
    public class OktaAppLinkDTO
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string LinkUrl { get; set; }
        public string LogoUrl { get; set; }
        public string AppName { get; set; }
        public string AppInstanceId { get; set; }
        public string AppAssignmentId { get; set; }
        public bool CredentialsSetup { get; set; }
        public bool Hidden { get; set; }
        public int SortOrder { get; set; }
    }
}
