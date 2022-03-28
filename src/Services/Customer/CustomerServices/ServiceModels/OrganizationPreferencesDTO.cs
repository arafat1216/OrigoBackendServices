using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public class OrganizationPreferencesDTO
    {
        public OrganizationPreferencesDTO(CustomerServices.Models.OrganizationPreferences organizationPreferences)
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

        /// <summary>
        ///     The organizations language preference, using the <c>ISO 639-1</c> standard.
        /// </summary>
        [RegularExpression("^[a-z]{2}")] // Exactly 2 lowercase characters
        public string PrimaryLanguage { get; set; }

        public short DefaultDepartmentClassification { get; set; }
    }
}
