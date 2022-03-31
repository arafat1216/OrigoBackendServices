using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace OrigoApiGateway.Services
{
    public class PartnerServices : IPartnerServices
    {
        public PartnerServices(ILogger<PartnerServices> logger, HttpClient httpClient,
            IOptions<PartnerConfiguration> options, IMapper mapper)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _mapper = mapper;
        }

        private readonly ILogger<PartnerServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly PartnerConfiguration _options;
        private readonly IMapper _mapper;

        public async Task<Partner> CreatePartnerAsync(Guid organizationId, Guid callerId)
        {
            try
            {
                var partnerDto = new CreatePartnerDto();
                partnerDto.CallerId = callerId;
                partnerDto.OrganizationId = organizationId;

                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", partnerDto);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save partner", (int)response.StatusCode);

                var partnerResponse = await response.Content.ReadFromJsonAsync<PartnerDto>();
                if (partnerResponse != null)
                {

                    var partner = new Partner
                    {
                        ExternalId = partnerResponse.ExternalId,
                        Organization = _mapper.Map<Organization>(partnerResponse.Organization)
                    };
                    return partner;
                }

                return null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "CreatePartnerAsync unknown error.");
                throw;
            }
        }

        public async Task<Partner> GetPartnerAsync(Guid partnerId)
        {
            try
            {
                var partner = await HttpClient.GetFromJsonAsync<PartnerDto>($"{ _options.ApiPath}/{partnerId}");
                if (partner == null)
                    return null;


                return new Partner { ExternalId = partner.ExternalId, Organization = _mapper.Map<Organization>(partner.Organization) };
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetPartnersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetPartnersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetPartnersAsync unknown error.");
                throw;
            }
        }

        public async Task<IList<Partner>> GetPartnersAsync()
        {
            try
            {
                var partnersFound = await HttpClient.GetFromJsonAsync<IList<PartnerDto>>($"{ _options.ApiPath}");
                if (partnersFound == null)
                    return null;

                List<Partner> partners = new List<Partner>();
                foreach (PartnerDto partnerDto in partnersFound)
                {
                    partners.Add(new Partner { ExternalId = partnerDto.ExternalId, Organization = _mapper.Map<Organization>(partnerDto.Organization) });
                }
                return partners;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "GetPartnersAsync failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "GetPartnersAsync failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "GetPartnersAsync unknown error.");
                throw;
            }
        }
    }
}
