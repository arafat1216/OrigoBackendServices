using AutoMapper;
using HardwareServiceOrderServices.ServiceModels;
using System.Threading.Tasks;

namespace HardwareServiceOrder.API.Mappings.Tests
{
    public class ApiCredentialProfileTests
    {
        private readonly IMapper _mapper;

        public ApiCredentialProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new HardwareServiceOrder.API.Mappings.ObscuredApiCredentialProfile());
            }).CreateMapper();
        }

        [Fact()]
        public void ApiCredentialProfileTest()
        {
            // Arrange

            ApiCredentialDTO apiCredentialDTO1 = new(1, (int)ServiceTypeEnum.SUR, "ApiUsername", "ApiPassword");
            ApiCredentialDTO apiCredentialDTO2 = new(1, (int)ServiceTypeEnum.SUR, null, null);
            ApiCredentialDTO apiCredentialDTO3 = new(1, (int)ServiceTypeEnum.SUR, string.Empty, string.Empty);

            // Act
            ViewModels.ObscuredApiCredential mappedWithValue = _mapper.Map<ViewModels.ObscuredApiCredential>(apiCredentialDTO1);
            ViewModels.ObscuredApiCredential mappedNullValue = _mapper.Map<ViewModels.ObscuredApiCredential>(apiCredentialDTO2);
            ViewModels.ObscuredApiCredential mappedEmptyStringValue = _mapper.Map<ViewModels.ObscuredApiCredential>(apiCredentialDTO3);

            /*
             * Assert
             */

            Assert.Equal(mappedWithValue.ServiceTypeId, mappedWithValue.ServiceTypeId);

            // Check username mapping
            Assert.True(mappedWithValue.ApiUsernameFilled);
            Assert.False(mappedNullValue.ApiUsernameFilled);
            Assert.False(mappedEmptyStringValue.ApiUsernameFilled);

            // Check password mapping
            Assert.True(mappedWithValue.ApiPasswordFilled);
            Assert.False(mappedNullValue.ApiPasswordFilled);
            Assert.False(mappedEmptyStringValue.ApiPasswordFilled);
        }
    }
}


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