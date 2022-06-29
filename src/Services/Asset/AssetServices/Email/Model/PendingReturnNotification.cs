﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
    public class PendingReturnNotification
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Origo Assets View Page Link
        /// </summary>
        public string AssetsLink { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public IList<string> Recipients { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Confirm Return";
        public const string TemplateName = "PendingReturn";
    }
}
