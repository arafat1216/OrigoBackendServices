using CustomerServices.Email;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class EmailServiceTests
    {
        public EmailServiceTests()
        {

        }

        [Fact]
        public void GetEmailTemplate()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService)));
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("EN"));
            Assert.NotNull(OrigoInvitationEnglishTemplate);
        }
    }
}
