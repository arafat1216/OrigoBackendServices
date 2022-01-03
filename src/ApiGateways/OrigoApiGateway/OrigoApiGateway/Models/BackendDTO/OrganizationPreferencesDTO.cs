namespace OrigoApiGateway.Models.BackendDTO
{
    public class OrganizationPreferencesDTO
    {
        public string WebPage { get; set; }
        public string LogoUrl { get; set; }
        public string OrganizationNotes { get; set; }
        public bool? EnforceTwoFactorAuth { get; set; }
        public string PrimaryLanguage { get; set; }
        public short? DefaultDepartmentClassification { get; set; }
    }
}
