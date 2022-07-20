using AutoMapper;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Services
{
    public class PartnerServices : IPartnerServices
    {
        public PartnerServices(ILogger<PartnerServices> logger, IHttpClientFactory httpClientFactory,
            IOptions<PartnerConfiguration> options, IMapper mapper)
        {
            _logger = logger;
            _options = options.Value;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        private readonly ILogger<PartnerServices> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("customerservices");
        private readonly PartnerConfiguration _options;
        private readonly IMapper _mapper;

        public async Task<Partner> CreatePartnerAsync(Guid organizationId, Guid callerId)
        {
            try
            {
                var partnerDto = new CreatePartnerDto { CallerId = callerId, OrganizationId = organizationId };
                var response = await HttpClient.PostAsJsonAsync($"{_options.ApiPath}", partnerDto);
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Unable to save partner", (int)response.StatusCode);

                var partnerResponse = await response.Content.ReadFromJsonAsync<PartnerDTO>();
                if (partnerResponse != null)
                {
                    var partner = new Partner
                    {
                        Id = partnerResponse.ExternalId,
                        Address = _mapper.Map<Address>(partnerResponse.Organization.Address),
                        ContactPerson = _mapper.Map<OrigoContactPerson>(partnerResponse.Organization.ContactPerson),
                        Name = partnerResponse.Organization.Name,
                        OrganizationNumber = partnerResponse.Organization.OrganizationNumber
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
                var partnerResponse = await HttpClient.GetFromJsonAsync<PartnerDTO>($"{ _options.ApiPath}/{partnerId}");

                if (partnerResponse == null)
                    return null;

                var partner = new Partner
                {
                    Id = partnerResponse.ExternalId,
                    Address = _mapper.Map<Address>(partnerResponse.Organization.Address),
                    ContactPerson = _mapper.Map<OrigoContactPerson>(partnerResponse.Organization.ContactPerson),
                    Name = partnerResponse.Organization.Name,
                    OrganizationNumber = partnerResponse.Organization.OrganizationNumber
                };

                return partner;
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
                var partnersFound = await HttpClient.GetFromJsonAsync<IList<PartnerDTO>>($"{ _options.ApiPath}");

                if (partnersFound == null)
                    return null;

                var partners = new List<Partner>();
                foreach (PartnerDTO partnerDto in partnersFound)
                {
                    var partner = new Partner
                    {
                        Id = partnerDto.ExternalId,
                        Address = _mapper.Map<Address>(partnerDto.Organization.Address),
                        ContactPerson = _mapper.Map<OrigoContactPerson>(partnerDto.Organization.ContactPerson),
                        Name = partnerDto.Organization.Name,
                        OrganizationNumber = partnerDto.Organization.OrganizationNumber
                    };
                    partners.Add(partner);
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
