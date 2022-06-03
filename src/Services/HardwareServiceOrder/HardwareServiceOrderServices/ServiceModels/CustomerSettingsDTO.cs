﻿using System.ComponentModel.DataAnnotations;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="Models.CustomerSettings"/>
    public class CustomerSettingsDTO
    {
        /// <inheritdoc cref="Models.CustomerSettings.ServiceId"/>
        public string? ServiceId { get; set; }

        /// <inheritdoc cref="Models.CustomerSettings.LoanDevicePhoneNumber"/>
        [Phone]
        [StringLength(maximumLength: 15)]
        [RegularExpression("^\\+[1-9]\\d{1,14}$")]
        public string? LoanDevicePhoneNumber { get; set; }

        /// <summary>
        ///     Backing field for <see cref="LoanDeviceEmail"/>.
        /// </summary>
        private string? _loanDeviceEmail { get; set; }

        /// <inheritdoc cref="Models.CustomerSettings.LoanDeviceEmail"/>
        [EmailAddress]
        [StringLength(maximumLength: 320)]
        public string? LoanDeviceEmail
        {
            get { return _loanDeviceEmail; }
            set { _loanDeviceEmail = value?.ToLowerInvariant(); }
        }

        /// <inheritdoc cref="Models.CustomerSettings.CustomerId"/>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Provider identifier
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// List of asset categories supported by the provider
        /// </summary>
        public List<int> AssetCategoryIds { get; set; }
    }
}
