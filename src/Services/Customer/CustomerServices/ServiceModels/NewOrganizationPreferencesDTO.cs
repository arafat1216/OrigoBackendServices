using System.ComponentModel.DataAnnotations;

#nullable enable

namespace CustomerServices.ServiceModels
{
    public class NewOrganizationPreferencesDTO
    {
        public NewOrganizationPreferencesDTO() { }

        public NewOrganizationPreferencesDTO(string? webPage, string? logoURL, string? notes, bool? enforceTwoFactorAuth, string primaryLanguage, short? defaultDepartmentClassification)
        {
            WebPage = webPage;
            LogoUrl = logoURL;
            OrganizationNotes = notes;
            EnforceTwoFactorAuth = enforceTwoFactorAuth;
            PrimaryLanguage = primaryLanguage;
            DefaultDepartmentClassification = defaultDepartmentClassification;
        }

        public string? WebPage { get; set; }
        public string? LogoUrl { get; set; }
        public string? OrganizationNotes { get; set; }
        public bool? EnforceTwoFactorAuth { get; set; }

        /// <summary>
        ///     The organizations language preference, using the <c>ISO 639-1</c> standard.
        /// </summary>
        /// <example>en</example>
        [RegularExpression("^[a-z]{2}")] // Exactly 2 lowercase characters
        public string PrimaryLanguage { get; set; }

        public short? DefaultDepartmentClassification { get; set; }

    }
}
