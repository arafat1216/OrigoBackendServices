using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

namespace OrigoApiGateway.Mappings.HardwareServiceOrder.Tests
{
    public class CustomerSettingsProfileTests
    {
        private readonly IMapper _mapper;

        public CustomerSettingsProfileTests()
        {
            _mapper = new MapperConfiguration(config =>
            {
                config.AddProfile(new CustomerSettingsProfile());
            }).CreateMapper();
        }


        [Fact(DisplayName = "Automapper profile: CustomerSettings to LoanDeviceSettings")]
        public void CustomerSettings_To_LoanDeviceSettings_ProfileTest()
        {
            // Arrange
            CustomerSettings customerSettings = new()
            {
                CustomerId = Guid.NewGuid(),
                ProvidesLoanDevice = true,
                LoanDeviceEmail = "test@email.com",
                LoanDevicePhoneNumber = "+4799999999"
            };

            // Act
            var result = _mapper.Map<LoanDeviceSettings>(customerSettings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerSettings.ProvidesLoanDevice, result.ProvidesLoanDevice);
            Assert.Equal(customerSettings.LoanDeviceEmail, result.LoanDeviceEmail);
            Assert.Equal(customerSettings.LoanDevicePhoneNumber, result.LoanDevicePhoneNumber);
        }
    }
}