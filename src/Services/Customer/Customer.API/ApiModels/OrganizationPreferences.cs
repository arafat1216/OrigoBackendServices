namespace Customer.API.ApiModels
{
    public class OrganizationPreferences
    {
        public OrganizationPreferences(CustomerServices.Models.OrganizationPreferences organizationPreferences)
        {
            WebPage = organizationPreferences.WebPage;
            LogoUrl = organizationPreferences.LogoUrl;
            OrganizationNotes = organizationPreferences.OrganizationNotes;
            EnforceTwoFactorAuth = organizationPreferences.EnforceTwoFactorAuth;
            PrimaryLanguage = organizationPreferences.PrimaryLanguage;
            DefaultDepartmentClassification = organizationPreferences.DefaultDepartmentClassification;
        }

        public string WebPage { get; set; }
        public string LogoUrl { get; set; }
        public string OrganizationNotes { get; set; }
        public bool EnforceTwoFactorAuth { get; set; }
        public string PrimaryLanguage { get; set; }
        public short DefaultDepartmentClassification { get; set; }
    }
}
