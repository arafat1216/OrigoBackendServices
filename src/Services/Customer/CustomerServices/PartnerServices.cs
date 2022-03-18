using CustomerServices.Infrastructure;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class PartnerServices : IPartnerServices
    {
        private readonly ILogger<PartnerServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public PartnerServices(ILogger<PartnerServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<Partner> CreatePartnerAsync(Partner partner)
        {
            return await _customerRepository.AddPartnerAsync(partner);
        }

        /// <summary>
        /// Return a partner with its corresponding organization entity
        /// </summary>
        /// <returns></returns>
        public async Task<Partner> GetPartnerAsync(Guid partnerId)
        {
            var partner = await _customerRepository.GetPartnerAsync(partnerId);
            if (partner != null)
            {
                partner.Organization.Preferences = await _customerRepository.GetOrganizationPreferencesAsync(partner.Organization.OrganizationId);
                partner.Organization.Location = await _customerRepository.GetOrganizationLocationAsync(partner.Organization.PrimaryLocation);
            }
           
            return partner;
        }

        /// <summary>
        /// Returns all partner objects along with their corresponding Organization entities
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Partner>> GetPartnersAsync()
        {
            var partners = await _customerRepository.GetPartnersAsync();
            foreach (Partner partner in partners)
            {
                partner.Organization.Preferences = await _customerRepository.GetOrganizationPreferencesAsync(partner.Organization.OrganizationId);
                partner.Organization.Location = await _customerRepository.GetOrganizationLocationAsync(partner.Organization.PrimaryLocation);
            }
            return partners;
        }
    }
}
