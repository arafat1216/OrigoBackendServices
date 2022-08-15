using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Mappings.Tests
{
    public class ApiCredentialProfileTests
    {
        private readonly IMapper _mapper;

        public ApiCredentialProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new HardwareServiceOrderServices.Mappings.ApiCredentialProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "'ApiCredential' EF Entity to 'ApiCredentialDTO' entity remapping")]
        public async Task ApiCredential_EfToDTO_Test()
        {
            // Arange
            ApiCredential apiCredential = new((int)ServiceProviderEnum.ConmodoNo, (int)ServiceTypeEnum.SUR, "[ApiUsername]", "[ApiPassword]");
            apiCredential.LastUpdateFetched = DateTimeOffset.Parse("2020-01-31");

            // Act
            var result = _mapper.Map<ApiCredentialDTO>(apiCredential);

            // Assert
            Assert.Equal(apiCredential.ApiPassword, result.ApiPassword);
            Assert.Equal(apiCredential.ApiUsername, result.ApiUsername);
            Assert.Equal(apiCredential.CustomerServiceProviderId, result.CustomerServiceProviderId);
            Assert.Equal(apiCredential.ServiceTypeId, result.ServiceTypeId);
            Assert.Equal(apiCredential.LastUpdateFetched, result.LastUpdateFetched);
        }
    }
}