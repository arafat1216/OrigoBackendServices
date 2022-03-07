using Common.Utilities;
using Xunit;

namespace Common.UnitTests
{
    public class PhoneNumberUtilityTest
    {
        [Fact]
        public void PhoneNumberUtility_Valid_For_NorwegianRegion()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("+4790909090", "NO");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Valid_For_WithoutRegionPrefix()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("90909090", "no");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Valid_For_WithSpacing()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("90 90 9 09 0", "nb");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Fails_For_WithToManyNumbers()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("909090900", "NO");

            Assert.False(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Valid_For_SwedishRegion()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("46 771 793 336", "se");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Invalid_WhenSwedishAndNoCountryCode()
        {
            
            var validation = PhoneNumberUtility.ValidatePhoneNumber("+46 771 793 336", null);

            Assert.False(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Fails_WrongPrefix_and_NoCountryCode()
        {

            var validation = PhoneNumberUtility.ValidatePhoneNumber("+47 771 793 336", null);

            Assert.False(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Valid_SwedishPhoneNumber()
        {

            var validation = PhoneNumberUtility.ValidatePhoneNumber("073 621 54 65", "SE");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Valid_NorwegianNB()
        {

            var validation = PhoneNumberUtility.ValidatePhoneNumber("41730873", "nb");

            Assert.True(validation);
        }
        [Fact]
        public void PhoneNumberUtility_Fails_EmptyString()
        {
            var validation = PhoneNumberUtility.ValidatePhoneNumber("", "nb");

            Assert.False(validation);
        }
    }
}