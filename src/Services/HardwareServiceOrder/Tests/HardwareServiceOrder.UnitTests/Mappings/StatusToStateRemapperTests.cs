namespace HardwareServiceOrderServices.Mappings.Tests
{
    public class StatusToStateRemapperTests
    {
        [Theory()]
        [InlineData(ServiceStatusEnum.CompletedRepairedOnWarranty, (int)ServiceStateEnum.Completed)]
        [InlineData(ServiceStatusEnum.OngoingUserActionNeeded, (int)ServiceStateEnum.Ongoing)]
        [InlineData(ServiceStatusEnum.Unknown, (int)ServiceStateEnum.Unknown)]
        [InlineData((ServiceStatusEnum)999, (int)ServiceStateEnum.Null)] // This input value don't exist in the enum
        public void GetIdTest(ServiceStatusEnum input, int expectedResult)
        {
            Assert.Equal(expectedResult, StatusToStateRemapper.GetId(input));
        }

        [Theory()]
        [InlineData((int)ServiceStatusEnum.CompletedRepairedOnWarranty, (int)ServiceStateEnum.Completed)]
        [InlineData((int)ServiceStatusEnum.OngoingUserActionNeeded, (int)ServiceStateEnum.Ongoing)]
        [InlineData((int)ServiceStatusEnum.Unknown, (int)ServiceStateEnum.Unknown)]
        [InlineData(999, (int)ServiceStateEnum.Null)] // This input value don't exist in the enum
        public void GetIdFromIdIdTest(int input, int expectedResult)
        {
            Assert.Equal(expectedResult, StatusToStateRemapper.GetId(input));
        }

        [Theory()]
        [InlineData(ServiceStatusEnum.CompletedRepairedOnWarranty, ServiceStateEnum.Completed)]
        [InlineData(ServiceStatusEnum.OngoingUserActionNeeded, ServiceStateEnum.Ongoing)]
        [InlineData(ServiceStatusEnum.Unknown, ServiceStateEnum.Unknown)]
        [InlineData((ServiceStatusEnum)999, ServiceStateEnum.Null)] // This input value don't exist in the enum

        public void GetEnumValueFromEnumTest(ServiceStatusEnum input, ServiceStateEnum expectedResult)
        {
            Assert.Equal(expectedResult, StatusToStateRemapper.GetEnumValue(input));
        }

        [Theory()]
        [InlineData((int)ServiceStatusEnum.CompletedRepairedOnWarranty, ServiceStateEnum.Completed)]
        [InlineData((int)ServiceStatusEnum.OngoingUserActionNeeded, ServiceStateEnum.Ongoing)]
        [InlineData((int)ServiceStatusEnum.Unknown, ServiceStateEnum.Unknown)]
        [InlineData(999, ServiceStateEnum.Null)] // This input value don't exist in the enum
        public void GetEnumValueFromIdTest(int input, ServiceStateEnum expectedResult)
        {
            Assert.Equal(expectedResult, StatusToStateRemapper.GetEnumValue(input));

        }
    }
}