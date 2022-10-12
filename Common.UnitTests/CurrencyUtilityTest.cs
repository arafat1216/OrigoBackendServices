using Common.Utilities;
using Xunit;

namespace Common.UnitTests
{
    public class CurrencyUtilityTest
    {
        [Fact]
        public void CurrencyUtility_Valid_NOK()
        {
            var validation = CurrencyUtility.Validate("NOK");
            Assert.True(validation);
        }

        [Fact]
        public void CurrencyUtility_Valid_USD()
        {
            var validation = CurrencyUtility.Validate("USD");
            Assert.True(validation);
        }

        [Fact]
        public void CurrencyUtility_Valid_EUR()
        {
            var validation = CurrencyUtility.Validate("EUR");
            Assert.True(validation);
        }

        [Fact]
        public void CurrencyUtility_Valid_CNY()
        {
            var validation = CurrencyUtility.Validate("CNY");
            Assert.True(validation);
        }

        [Fact]
        public void CurrencyUtility_Invalid()
        {
            var validation = CurrencyUtility.Validate("KKK");
            Assert.False(validation);
        }

        [Fact]
        public void CurrencyUtility_GetCurrency_Valid_Norway()
        {
            var currency = CurrencyUtility.GetCurrencyCode("no");
            Assert.NotEmpty(currency);
        }

        [Fact]
        public void CurrencyUtility_GetCurrency_Valid_Sweden()
        {
            var currency = CurrencyUtility.GetCurrencyCode("se");
            Assert.NotEmpty(currency);
        }

        [Fact]
        public void CurrencyUtility_GetCurrency_Invalid()
        {
            var currency = CurrencyUtility.GetCurrencyCode("vv");
            Assert.Empty(currency);
        }
    }
}