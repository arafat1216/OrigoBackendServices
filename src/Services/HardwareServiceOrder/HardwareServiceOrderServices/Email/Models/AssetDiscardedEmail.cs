﻿namespace HardwareServiceOrderServices.Email.Models
{
    public class AssetDiscardedEmail
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Replace Discarded Asset";
        public const string TemplateName = "AssetDiscardedEmail";
    }
}