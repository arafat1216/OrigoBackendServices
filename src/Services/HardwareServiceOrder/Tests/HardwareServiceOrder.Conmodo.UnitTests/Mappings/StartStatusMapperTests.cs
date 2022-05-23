using HardwareServiceOrderServices.Conmodo.Mappings;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrder.Conmodo.UnitTests.Mappings
{
    /// <summary>
    ///     Tests for the <see cref="StartStatusMapper"/> class.
    /// </summary>
    public class StartStatusMapperTests
    {
        private StartStatusMapper Mapper { get; } = new();

        /// <summary>
        ///     Positive tests for <see cref="StartStatusMapper.FromServiceType(int)"/>.
        /// </summary>
        /// <param name="serviceTypeId"> Our serviceTypeID that we are mapping from. </param>
        /// <param name="expectedStartStatus"> The expected Conmodo start-status ID (<see cref="int"/>). </param>
        /// <param name="expectedStartStatusName"> The expected Conmodo start-status name (<see cref="string"/>). </param>
        /// <returns> A <see cref="Task"/> containing the unit-test execution. </returns>
        [Theory]
        [InlineData((int)ServiceTypeEnum.SUR, 25007, "Garanti?")]
        public void FromServiceType1(int serviceTypeId, int expectedStartStatus, string expectedStartStatusName)
        {
            var actual = Mapper.FromServiceType(serviceTypeId);

            Assert.Equal(expectedStartStatus, actual.StartStatusID);
            Assert.Equal(expectedStartStatusName, actual.StartStatusName);
        }


        /// <summary>
        ///     Negative tests for <see cref="StartStatusMapper.FromServiceType(int)"/> checking that it throws exceptions when it should.
        /// </summary>
        /// <param name="serviceTypeId"> Our serviceTypeID that we are mapping from. </param>
        /// <returns> A <see cref="Task"/> containing the unit-test execution. </returns>
        [Theory]
        [InlineData((int)ServiceTypeEnum.Null)] // enum default value
        [InlineData(new int())] // int default value
        [InlineData(-1)]
        [InlineData(99999999)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1025:InlineData should be unique within the Theory it belongs to", Justification = "We want to check against both the default int value, and the null value in the enum in case they end up different.")]
        public void FromServiceType2(int serviceTypeId)
        {
            Assert.Throws<NotSupportedException>(() => Mapper.FromServiceType(serviceTypeId));
        }
    }
}
