﻿using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace CustomerServices
{
    public class PartnerServices : IPartnerServices
    {
        private readonly ILogger<PartnerServices> _logger;
        private readonly IOrganizationRepository _organizationRepository;

        public PartnerServices(ILogger<PartnerServices> logger, IOrganizationRepository organizationRepository)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     The organization cannot be a child, and can't be attached to another partner. Once created, the organization will
        ///     be set as a customer of itself.
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CustomerNotFoundException"></exception>
        /// <exception cref="DuplicateException"></exception>
        public async Task<Partner> CreatePartnerAsync(Guid organizationId)
        {
            var existingOrganization = await _organizationRepository.GetOrganizationAsync(organizationId, includeDepartments: false, customersOnly: false);

            if (existingOrganization is null)
                throw new CustomerNotFoundException("The organization was not found.");
            if (existingOrganization.ParentId is not null)
                throw new ArgumentException("A partner-organization cannot have parent organizations.");
            if (existingOrganization.Partner is not null || existingOrganization.IsCustomer)
                throw new ArgumentException("This organization can't be set as a partner. It is either a customer, or already has a partner assigned.");
            if (await _organizationRepository.OrganizationIsPartner(existingOrganization.Id))
                throw new DuplicateException("This organization is already a partner.");

            var partner = new Partner(existingOrganization);
            var partnerResult = await _organizationRepository.AddPartnerAsync(partner);
            existingOrganization.ChangePartner(partner);
            await _organizationRepository.SaveEntitiesAsync();

            return partnerResult;
        }

        public async Task<Partner?> GetPartnerAsync(Guid partnerId, bool includeOrganization)
        {
            var partner = await _organizationRepository.GetPartnerAsync(partnerId, includeOrganization, asNoTracking: true);
            return partner;
        }

        public async Task<IList<Partner>> GetPartnersAsync()
        {
            var partners = await _organizationRepository.GetPartnersAsync(true);

            return partners;
        }
    }
}
