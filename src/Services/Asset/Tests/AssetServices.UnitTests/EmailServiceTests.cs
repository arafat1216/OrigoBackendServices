using AssetServices.Email;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xunit;

namespace AssetServices.UnitTests
{
    public class EmailServiceTests
    {
        public EmailServiceTests()
        {

        }

        [Fact]
        public void GetEmailTemplate()
        {
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));
            var ReassignedToUserEngTemplate = resourceManger.GetString("ReassignedToUser", CultureInfo.CreateSpecificCulture("EN"));
            var ReportAssetEngTemplate = resourceManger.GetString("ReportAsset", CultureInfo.CreateSpecificCulture("EN"));
            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
        }
    }
}
