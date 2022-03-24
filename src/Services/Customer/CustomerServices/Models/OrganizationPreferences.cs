using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;

namespace CustomerServices.Models
{
    /// <summary>
    ///     Represents the settings and preferences for a single <see cref="Organization"/>.
    /// </summary>
    public class OrganizationPreferences : Entity
    {
        public Guid OrganizationId { get; set; }
        public string WebPage { get; protected set; }
        public string LogoUrl { get; protected set; }
        public string OrganizationNotes { get; protected set; }
        public bool EnforceTwoFactorAuth { get; protected set; }
        public string PrimaryLanguage { get; protected set; }
        public short DefaultDepartmentClassification { get; protected set; }


        /// <summary>
        /// Added to prevent entity framework No suitable constructor found exception.
        /// </summary>
        protected OrganizationPreferences()
        { }

        public OrganizationPreferences(Guid organizationId, Guid callerId, string webPage, string logoUrl, string organizationNotes, bool enforceTwoFactorAuth, string primaryLanguage, short defaultDepartmentClassification)
        {
            OrganizationId = organizationId;
            WebPage = webPage;
            LogoUrl = logoUrl;
            OrganizationNotes = organizationNotes;
            EnforceTwoFactorAuth = enforceTwoFactorAuth;
            PrimaryLanguage = primaryLanguage;
            DefaultDepartmentClassification = defaultDepartmentClassification;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            AddDomainEvent(new OrganizationPreferencesAddedDomainEvent(this));
        }

        /// <summary>
        /// Cannot check for null in constructor, since patch method needs to check if Preferences object to update has fields we wish to ignore
        /// This method allows us to set null fields to String.Empty for update method, while ignoring null fields for patch method.
        /// </summary>
        public void SetFieldsToEmptyIfNull()
        {
            if (WebPage == null) WebPage = "";
            if (LogoUrl == null) LogoUrl = "";
            if (OrganizationNotes == null) OrganizationNotes = "";
            if (PrimaryLanguage == null) PrimaryLanguage = "";
        }

        public void UpdatePreferences(OrganizationPreferences newPreferences)
        {
            bool isUpdated = false;

            if (WebPage != newPreferences.WebPage)
            {
                string oldWebPage = WebPage;
                WebPage = newPreferences.WebPage;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedWebPageDomainEvent(this, oldWebPage));
            }

            if (LogoUrl != newPreferences.LogoUrl)
            {
                string oldLogoUrl = LogoUrl;
                LogoUrl = newPreferences.LogoUrl;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedLogoUrlDomainEvent(this, oldLogoUrl));
            }

            if (OrganizationNotes != newPreferences.OrganizationNotes)
            {
                string oldOrganizationNotes = OrganizationNotes;
                OrganizationNotes = newPreferences.OrganizationNotes;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedOrganizationNotesDomainEvent(this, oldOrganizationNotes));
            }

            if (EnforceTwoFactorAuth != newPreferences.EnforceTwoFactorAuth)
            {
                string oldEnforceTwoFactorAuth = EnforceTwoFactorAuth.ToString();
                EnforceTwoFactorAuth = newPreferences.EnforceTwoFactorAuth;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedEnforceTwoFactorAuthDomainEvent(this, oldEnforceTwoFactorAuth));
            }

            if (PrimaryLanguage != newPreferences.PrimaryLanguage)
            {
                string oldPrimaryLanguage = PrimaryLanguage;
                PrimaryLanguage = newPreferences.PrimaryLanguage;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedPrimaryLanguageDomainEvent(this, oldPrimaryLanguage));
            }

            if (DefaultDepartmentClassification != newPreferences.DefaultDepartmentClassification)
            {
                string oldDefaultDepartmentClassification = DefaultDepartmentClassification.ToString();
                DefaultDepartmentClassification = newPreferences.DefaultDepartmentClassification;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedDefaultDepartmentClassificationDomainEvent(this, oldDefaultDepartmentClassification));
            }

            if (isUpdated)
            {
                LastUpdatedDate = DateTime.UtcNow;
                UpdatedBy = newPreferences.UpdatedBy;
            }
        }

        public void PatchPreferences(OrganizationPreferences newPreferences)
        {
            bool isUpdated = false;
            if (WebPage != newPreferences.WebPage && newPreferences.WebPage != null)
            {
                string oldWebPage = WebPage;
                WebPage = newPreferences.WebPage;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedWebPageDomainEvent(this, oldWebPage));

            }
            if (LogoUrl != newPreferences.LogoUrl && newPreferences.LogoUrl != null)
            {
                string oldLogoUrl = LogoUrl;
                LogoUrl = newPreferences.LogoUrl;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedLogoUrlDomainEvent(this, oldLogoUrl));
            }
            if (OrganizationNotes != newPreferences.OrganizationNotes && newPreferences.OrganizationNotes != null)
            {
                string oldOrganizationNotes = OrganizationNotes;
                OrganizationNotes = newPreferences.OrganizationNotes;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedOrganizationNotesDomainEvent(this, oldOrganizationNotes));
            }
            if (EnforceTwoFactorAuth != newPreferences.EnforceTwoFactorAuth)
            {
                string oldEnforceTwoFactorAuth = EnforceTwoFactorAuth.ToString();
                EnforceTwoFactorAuth = newPreferences.EnforceTwoFactorAuth;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedEnforceTwoFactorAuthDomainEvent(this, oldEnforceTwoFactorAuth));
            }
            if (PrimaryLanguage != newPreferences.PrimaryLanguage && newPreferences.PrimaryLanguage != null)
            {
                string oldPrimaryLanguage = PrimaryLanguage;
                PrimaryLanguage = newPreferences.PrimaryLanguage;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedPrimaryLanguageDomainEvent(this, oldPrimaryLanguage));
            }
            if (DefaultDepartmentClassification != newPreferences.DefaultDepartmentClassification)
            {
                string oldDefaultDepartmentClassification = DefaultDepartmentClassification.ToString();
                DefaultDepartmentClassification = newPreferences.DefaultDepartmentClassification;
                isUpdated = true;
                AddDomainEvent(new OrganizationPreferencesChangedDefaultDepartmentClassificationDomainEvent(this, oldDefaultDepartmentClassification));
            }
            if (isUpdated)
            {
                LastUpdatedDate = DateTime.UtcNow;
                UpdatedBy = newPreferences.UpdatedBy;
            }
        }

        public void Delete(Guid callerId)
        {
            IsDeleted = true;
            LastUpdatedDate = DateTime.UtcNow;
            UpdatedBy = callerId;
            AddDomainEvent(new OrganizationPreferencesDeletedDomainEvent(this));
        }
    }
}
