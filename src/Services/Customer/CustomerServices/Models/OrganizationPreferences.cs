using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class OrganizationPreferences : Entity
    {
        public Guid OrganizationId { get; set; }
        public Guid CreatedBy { get; protected set; }
        public Guid UpdatedBy { get; protected set; }
        public string WebPage { get; protected set; }
        public string LogoUrl { get; protected set; }
        public string OrganizationNotes { get; protected set; }
        public bool EnforceTwoFactorAuth { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public string PrimaryLanguage { get; protected set; }
        public short DefaultDepartmentClassification { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }


        /// <summary>
        /// Added to prevent entity framework No suitable constructor found exception.
        /// </summary>
        protected OrganizationPreferences()
        { }

        public OrganizationPreferences(Guid organizationId, Guid callerId, string webPage, string logoUrl, string organizationNotes, bool enforceTwoFactorAuth, string primaryLanguage, short defaultDepartmentClassification)
        {
            OrganizationId = organizationId;
            WebPage = (webPage == null) ? "" : webPage;
            LogoUrl = (logoUrl == null) ? "" : logoUrl;
            OrganizationNotes = (organizationNotes == null) ? "" : organizationNotes;
            EnforceTwoFactorAuth = enforceTwoFactorAuth;
            PrimaryLanguage = (primaryLanguage == null) ? "" : primaryLanguage;
            DefaultDepartmentClassification = defaultDepartmentClassification;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public void UpdatePreferences(OrganizationPreferences newPreferences)
        {
            WebPage = newPreferences.WebPage;
            LogoUrl = newPreferences.LogoUrl;
            OrganizationNotes = newPreferences.OrganizationNotes;
            EnforceTwoFactorAuth = newPreferences.EnforceTwoFactorAuth;
            PrimaryLanguage = newPreferences.PrimaryLanguage;
            DefaultDepartmentClassification = newPreferences.DefaultDepartmentClassification;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = newPreferences.UpdatedBy;
        }

        public void PatchPreferences(OrganizationPreferences newPreferences)
        {
            bool isUpdated = false;
            if (WebPage != newPreferences.WebPage)
            {
                WebPage = newPreferences.WebPage;
                isUpdated = true;
            }
            if (LogoUrl != newPreferences.LogoUrl)
            {
                LogoUrl = newPreferences.LogoUrl;
                isUpdated = true;
            }
            if (OrganizationNotes != newPreferences.OrganizationNotes)
            {
                OrganizationNotes = newPreferences.OrganizationNotes;
                isUpdated = true;
            }
            if (EnforceTwoFactorAuth != newPreferences.EnforceTwoFactorAuth)
            {
                EnforceTwoFactorAuth = newPreferences.EnforceTwoFactorAuth;
                isUpdated = true;
            }
            if (PrimaryLanguage != newPreferences.PrimaryLanguage)
            {
                PrimaryLanguage = newPreferences.PrimaryLanguage;
                isUpdated = true;
            }
            if (DefaultDepartmentClassification != newPreferences.DefaultDepartmentClassification)
            {
                DefaultDepartmentClassification = newPreferences.DefaultDepartmentClassification;
                isUpdated = true;
            }
            if (isUpdated)
            {
                UpdatedAt = DateTime.UtcNow;
                UpdatedBy = newPreferences.UpdatedBy;
            }
        }

        public void Delete(Guid callerId)
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = callerId;
        }
    }
}
