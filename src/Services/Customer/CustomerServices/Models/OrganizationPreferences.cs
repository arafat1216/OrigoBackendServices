﻿using Common.Seedwork;
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
            WebPage = webPage;
            LogoUrl = logoUrl;
            OrganizationNotes = organizationNotes;
            EnforceTwoFactorAuth = enforceTwoFactorAuth;
            PrimaryLanguage = primaryLanguage;
            DefaultDepartmentClassification = defaultDepartmentClassification;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }
    }
}
