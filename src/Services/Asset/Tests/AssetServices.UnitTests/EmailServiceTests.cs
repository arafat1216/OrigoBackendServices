using AssetServices.Email;
using AssetServices.Email.Model;
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
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture("EN"));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture("EN"));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture("EN"));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture("EN"));

            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
        }

    }
}
