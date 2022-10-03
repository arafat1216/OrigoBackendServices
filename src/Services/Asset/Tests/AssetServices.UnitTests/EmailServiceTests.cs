using AssetServices.Email;
using AssetServices.Email.Model;
using System.Collections.Generic;
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
        public void GetEmailTemplate_English()
        {
            var language = "en";

            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nAn asset was successfully reassigned from your department.\r\n\r\n[View Asset]({{AssetLink}})", ReassignedToUserEngTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nAn asset has been reported {{ReportType}}.and has been set to inactive.\r\n\r\n### Asset:\r\n{{AssetName}} ({{AssetId})\r\n### Report Date:\r\n{{ReportDate}}\r\n### Reported by:\r\n{{ReportedBy}}\r\n### What happened with the asset?\r\n{{ReportType}}\r\n### How did it happen?\r\n{{Description}}\r\n### In What time period did it happen?\r\n{{DateFrom}} To {{DateTo}}\r\n### Where did it happen?\r\n{{Address}}", ReportAssetEngTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nIn regards of your offboarding, {{ManagerName}} has chosen buyout on the following asset:\r\n\r\n### Asset:\r\n{{AssetName}} ({{AssetId})\r\n\r\n### Buyout Date:\r\n{{BuyoutDate}}\r\n\r\n### Buyout Price:\r\n{{BuyoutPrice}} {{Currency}}\r\n\r\nThis means you will get a salary deduction of {{BuyoutPrice}} {{Currency}}, and the asset will be of your ownership.\r\n\r\nImportant! Please remember to delete all company files on your last working day.", ManagerOnBehalfBuyoutEngTemplate);
            Assert.Equal("Asset Buyout", ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.Equal("Hello {{FirstName}}!  \r\n\r\nYour Manager has chosen Return on the following asset:  \r\n\r\n### Asset: {{AssetName}} ({{AssetId})  \r\n### Date: {{ReturnDate}}  \r\n\r\nImportant! Please remember to delete all company files from your device.", ManagerOnBehalfReturnEngTemplate);
            Assert.Equal("Asset Return", ManagerOnBehalfReturnEngSubTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Norwegian()
        {
            var language = "no";
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nEn eiendel ble overført fra avdelingen din.\r\n\r\n[Se eiendel]({{AssetLink}})", ReassignedToUserEngTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nEn eiendel har blitt rapportert {{ReportType}}. Og har blitt satt til inaktiv.\r\n\r\n### Eiendel:\r\n{{AssetName}} ({{AssetId})\r\n### Rapportdato:\r\n{{ReportDate}}\r\n### Rapportert av:\r\n{{ReportedBy}}\r\n### Hva skjedde med eiendelen?\r\n{{ReportType}}\r\n### Hvordan skjedde det?\r\n{{Description}}\r\n### I hvilken tidsperiode skjedde det?\r\n{{DateFrom}} Til {{DateTo}}\r\n### Hvor skjedde det?\r\n{{Addresse}}", ReportAssetEngTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nNår det gjelder offboardingen din, har {{ManagerName}} valgt buyout på følgende eiendel:\r\n\r\n### Eiendel:\r\n{{AssetName}} ({{AssetId})\r\n\r\n### Utkjøpsdato:\r\n{{BuyoutDate}}\r\n\r\n### Utkjøpspris:\r\n{{BuyoutPrice}} {{Currency}}\r\n\r\nDette betyr at du får et lønnstrekk på {{BuyoutPrice}} {{Currency}}, og eiendelen vil eies av deg.\r\n\r\nViktig! Husk å slette alle firmafiler på eidendelen siste arbeidsdag.", ManagerOnBehalfBuyoutEngTemplate);
            Assert.Equal("Oppkjøp av eiendeler", ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.Equal("Hei {{FirstName}}!\r\n\r\nDin leder har valgt avkastning på følgende eiendel:\r\n\r\n### Eidendel: {{AssetName}} ({{AssetId})\r\n### Dato: {{ReturnDate}}\r\n\r\nViktig! Husk å slette alle firmafiler fra enheten din.", ManagerOnBehalfReturnEngTemplate);
            Assert.Equal("Retur av eiendel", ManagerOnBehalfReturnEngSubTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Swedish()
        {
            var language = "sv";
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nEn tillgång har tilldelats om från din avdelning.\r\n\r\n[Visa tillgång]({{AssetLink}})", ReassignedToUserEngTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nEn tillgång har rapporterats {{ReportType}}. Och har ställts in på inaktiv.\r\n\r\n### Tillgång:\r\n{{AssetName}} ({{AssetId})\r\n### Rapportdatum:\r\n{{ReportDate}}\r\n### Rapporterad av:\r\n{{ReportedBy}}\r\n### Vad hände med tillgången?\r\n{{ReportType}}\r\n### Hur hände det?\r\n{{Description}}\r\n### Under vilken tidsperiod hände det?\r\n{{DateFrom}} Till {{DateTo}}\r\n### Var hände det?\r\n{{Addresse}}", ReportAssetEngTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nNär det gäller din offboarding har {{ManagerName}} valt buyout på följande tillgång:\r\n\r\n### Tillgång:\r\n{{AssetName}} ({{AssetId})\r\n\r\n### Utköpsdatum:\r\n{{BuyoutDate}}\r\n\r\n### Utköpspris:\r\n{{BuyoutPrice}} {{Currency}}\r\n\r\nDet betyder att du får ett löneavdrag på {{BuyoutPrice}} {{Currency}}, och tillgången kommer att äga dig.\r\n\r\nViktig! Kom ihåg att radera alla företagsfiler på din sista arbetsdag.", ManagerOnBehalfBuyoutEngTemplate);
            Assert.Equal("Utköp av tillgångar", ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.Equal("Hej {{FirstName}}!\r\n\r\nDin chef har valt avkastning på följande tillgång:\r\n\r\n### Tillgång: {{AssetName}} ({{AssetId})\r\n### Datum: {{ReturnDate}}\r\n\r\nViktig! Kom ihåg att ta bort alla företagsfiler från din enhet.", ManagerOnBehalfReturnEngTemplate);
            Assert.Equal("Retur av tillgång", ManagerOnBehalfReturnEngSubTemplate);
        }

    }
}
