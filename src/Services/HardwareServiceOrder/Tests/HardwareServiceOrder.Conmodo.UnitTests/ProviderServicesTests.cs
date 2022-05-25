using HardwareServiceOrderServices.Conmodo;

namespace HardwareServiceOrder.Conmodo.UnitTests
{
    /// <summary>
    ///     Tests for the <see cref="ConmodoProviderServices"/> class.
    /// </summary>
    public class ProviderServicesTests
    {
        /// <summary>
        ///     A strict, pre-configured mock that is configured with <see cref="SetupAllMockResponses(ref Mock{IApiRequests})"/>,
        ///     and using the seeded fields.
        /// </summary>
        private readonly Mock<IApiRequests> _mock;


        public ProviderServicesTests()
        {
            _mock = new(MockBehavior.Strict);
            SetupAllMockResponses(ref _mock);
        }


        /*
         * Methods that sets up/configures mocks. The mock uses the seed-data fields, and sets it up to be returned when a valid parameter value is called.
         */

        private void SetupAllMockResponses(ref Mock<IApiRequests> mock)
        {
            Setup_UpdatedOrdersResponse(ref mock);
            Setup_OrderResponse1(ref mock);
            Setup_OrderResponse2(ref mock);
            Setup_OrderResponse3(ref mock);
            Setup_TestResponse(ref mock);
        }

        private void Setup_UpdatedOrdersResponse(ref Mock<IApiRequests> mock)
        {
            mock.Setup(p => p.GetUpdatedOrdersAsync(It.Is<DateTimeOffset?>(m => !m.HasValue || m.Value < DateTimeOffset.Parse("2022-01-01"))))
                .ReturnsAsync(SeededConmodoModels.UpdatedOrdersResponse);
        }

        private void Setup_OrderResponse1(ref Mock<IApiRequests> mock)
        {
            mock.Setup(p => p.GetOrderAsync(It.Is<string>(m => m.ToLower() == "082c9ae0-ab03-4175-92a7-27d5a791cedc")))
                .ReturnsAsync(SeededConmodoModels.OrderResponse1);
        }

        private void Setup_OrderResponse2(ref Mock<IApiRequests> mock)
        {
            mock.Setup(p => p.GetOrderAsync(It.Is<string>(m => m == "NOLF693-115")))
                .ReturnsAsync(SeededConmodoModels.OrderResponse2);
        }

        private void Setup_OrderResponse3(ref Mock<IApiRequests> mock)
        {
            mock.Setup(p => p.GetOrderAsync(It.Is<string>(m => m.ToLower() == "1e45c7b6-8000-4ca4-a6c5-c5dc41ac4f63")))
                .ReturnsAsync(SeededConmodoModels.OrderResponse3);
        }

        private void Setup_TestResponse(ref Mock<IApiRequests> mock)
        {
            mock.Setup(p => p.TestAsync())
                .ReturnsAsync("Ok!");
        }


        /*
         * Tests
         */

        [Fact]
        public async Task TestCallAsync()
        {
            Mock<IApiRequests> mock = new(MockBehavior.Strict);
            SetupAllMockResponses(ref mock);

            ConmodoProviderServices providerServices = new(_mock.Object);
            var actual = await providerServices.TestCall();

            // Assert
            Assert.Equal("Ok!", actual);
        }


        /// <summary>
        ///     Positive tests for <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>. 
        ///     Tests the validation of the input-parameters.
        /// </summary>
        /// <param name="serviceProviderOrderId1"></param>
        /// <param name="serviceProviderOrderId2"></param>
        /// <returns> A <see cref="Task"/> containing the unit-test execution. </returns>
        /// <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>
        [Theory]
        [InlineData("082c9ae0-ab03-4175-92a7-27d5a791cedc", null)]
        [InlineData("NOLF693-115", null)]
        [InlineData("NOLF693", null)]
        [InlineData("1e45c7b6-8000-4ca4-a6c5-c5dc41ac4f63", null)]
        [InlineData("1e45c7b6-8000-4ca4-a6c5-c5dc41ac4f63", "10115186")]
        public async Task GetRepairOrderAsync1(string serviceProviderOrderId1, string? serviceProviderOrderId2)
        {
            Mock<IApiRequests> mock = new();
            mock.Setup(p => p.GetOrderAsync(It.IsAny<string>()))
                .ReturnsAsync(SeededConmodoModels.OrderResponse1);

            ConmodoProviderServices providerServices = new(mock.Object);
            var actual = await providerServices.GetRepairOrderAsync(serviceProviderOrderId1, serviceProviderOrderId2);

            // Assert
            Assert.NotNull(actual);
        }


        /// <summary>
        ///     Negative tests for <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>. 
        ///     Tests the validation of the input-parameters to make sure it correctly throws exceptions.
        /// </summary>
        /// <param name="serviceProviderOrderId1"></param>
        /// <param name="serviceProviderOrderId2"></param>
        /// <returns> A <see cref="Task"/> containing the unit-test execution. </returns>
        /// <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>
        [Theory]
        [InlineData("082c9ae0-ab03-4175-92a7-27d5a791cedc", "")] // Invalid int value
        [InlineData("082c9ae0-ab03-4175-92a7-27d5a791cedc", "a")] // Invalid int value
        [InlineData("", null)] // Not a GUID or NOLF value
        [InlineData("invalid", null)] // Not a GUID or NOLF value
        [InlineData("NOLF123--123", null)] // Invalid NOLF format
        [InlineData("NOLF123a", null)] // Invalid NOLF format
        [InlineData("NOLF", null)] // Invalid NOLF format
        [InlineData("j83a2ae0-fb53-6133-24c7-90d5a791aacb", null)] // Invalid GUID
        public async Task GetRepairOrderAsync2(string serviceProviderOrderId1, string? serviceProviderOrderId2)
        {
            Mock<IApiRequests> mock = new(MockBehavior.Strict);
            SetupAllMockResponses(ref mock);

            ConmodoProviderServices providerServices = new(mock.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await providerServices.GetRepairOrderAsync(serviceProviderOrderId1, serviceProviderOrderId2));
        }


        /// <summary>
        ///     Positive tests for <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>. 
        ///     Tests the actual functionality.
        /// </summary>
        /// <returns> A <see cref="Task"/> containing the unit-test execution. </returns>
        /// <see cref="ConmodoProviderServices.GetRepairOrderAsync(string, string?)"/>
        [Fact(Skip = "Not implemented")]
        public void GetRepairOrderAsync3()
        {
            throw new NotImplementedException();
        }


        [Fact(Skip = "Not implemented")]
        public void GetUpdatedRepairOrdersAsync()
        {
            throw new NotImplementedException();

            Mock<IApiRequests> mock = new(MockBehavior.Strict);
            SetupAllMockResponses(ref mock);

            ConmodoProviderServices providerServices = new(mock.Object);

            //providerServices.GetUpdatedRepairOrdersAsync();
        }


        [Fact(Skip = "Not implemented")]
        public void CreateRepairOrderAsync()
        {
            throw new NotImplementedException();

            Mock<IApiRequests> mock = new(MockBehavior.Strict);
            SetupAllMockResponses(ref mock);

            ConmodoProviderServices providerServices = new(mock.Object);

            //providerServices.CreateRepairOrderAsync();
        }
    }
}