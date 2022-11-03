using AssetServices.Email;
using AssetServices.Email.Model;
using System.Globalization;
using System.Reflection;
using System.Resources;

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
            //Arrange
            var language = "en";
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!An asset was successfully reassigned from your department.[View Asset]({{AssetLink}})", ReassignedToUserEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!An asset has been reported {{ReportType}}.and has been set to inactive.### Asset:{{AssetName}} ({{AssetId})### Report Date:{{ReportDate}}### Reported by:{{ReportedBy}}### What happened with the asset?{{ReportType}}### How did it happen?{{Description}}### In What time period did it happen?{{DateFrom}} To {{DateTo}}### Where did it happen?{{Address}}", ReportAssetEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!In regards of your offboarding, {{ManagerName}} has chosen buyout on the following asset:### Asset:{{AssetName}} ({{AssetId})### Buyout Date:{{BuyoutDate}}### Buyout Price:{{BuyoutPrice}} {{Currency}}This means you will get a salary deduction of {{BuyoutPrice}} {{Currency}}, and the asset will be of your ownership.Important! Please remember to delete all company files on your last working day.", ManagerOnBehalfBuyoutEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Asset Buyout", ManagerOnBehalfBuyoutEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Hello {{FirstName}}!  Your Manager has chosen Return on the following asset:  ### Asset: {{AssetName}} ({{AssetId})  ### Date: {{ReturnDate}}  Important! Please remember to delete all company files from your device.", ManagerOnBehalfReturnEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Asset Return", ManagerOnBehalfReturnEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Norwegian()
        {
            //Arrange
            var language = "no";
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n\r\n which leads to unit test failing in docker 
            Assert.Equal("### Hei {{FirstName}}!En eiendel ble overført fra avdelingen din.[Se eiendel]({{AssetLink}})", ReassignedToUserEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!En eiendel har blitt rapportert {{ReportType}}. Og har blitt satt til inaktiv.### Eiendel:{{AssetName}} ({{AssetId})### Rapportdato:{{ReportDate}}### Rapportert av:{{ReportedBy}}### Hva skjedde med eiendelen?{{ReportType}}### Hvordan skjedde det?{{Description}}### I hvilken tidsperiode skjedde det?{{DateFrom}} Til {{DateTo}}### Hvor skjedde det?{{Addresse}}", ReportAssetEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!Når det gjelder offboardingen din, har {{ManagerName}} valgt buyout på følgende eiendel:### Eiendel:{{AssetName}} ({{AssetId})### Utkjøpsdato:{{BuyoutDate}}### Utkjøpspris:{{BuyoutPrice}} {{Currency}}Dette betyr at du får et lønnstrekk på {{BuyoutPrice}} {{Currency}}, og eiendelen vil eies av deg.Viktig! Husk å slette alle firmafiler på eidendelen siste arbeidsdag.", ManagerOnBehalfBuyoutEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Oppkjøp av eiendeler", ManagerOnBehalfBuyoutEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Hei {{FirstName}}!Din leder har valgt avkastning på følgende eiendel:### Eidendel: {{AssetName}} ({{AssetId})### Dato: {{ReturnDate}}Viktig! Husk å slette alle firmafiler fra enheten din.", ManagerOnBehalfReturnEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Retur av eiendel", ManagerOnBehalfReturnEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Swedish()
        {
            //Arrange
            var language = "sv";
            var resourceManger = new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var ReassignedToUserEngTemplate = resourceManger.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ReportAssetEngTemplate = resourceManger.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfBuyoutEngSubTemplate = resourceManger.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var ManagerOnBehalfReturnEngSubTemplate = resourceManger.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(ReassignedToUserEngTemplate);
            Assert.NotNull(ReportAssetEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngTemplate);
            Assert.NotNull(ManagerOnBehalfBuyoutEngSubTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngTemplate);
            Assert.NotNull(ManagerOnBehalfReturnEngSubTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n\r\n which leads to unit test failing in docker 
            Assert.Equal("### Hej {{FirstName}}!En tillgång har tilldelats om från din avdelning.[Visa tillgång]({{AssetLink}})", ReassignedToUserEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!En tillgång har rapporterats {{ReportType}}. Och har ställts in på inaktiv.### Tillgång:{{AssetName}} ({{AssetId})### Rapportdatum:{{ReportDate}}### Rapporterad av:{{ReportedBy}}### Vad hände med tillgången?{{ReportType}}### Hur hände det?{{Description}}### Under vilken tidsperiod hände det?{{DateFrom}} Till {{DateTo}}### Var hände det?{{Addresse}}", ReportAssetEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!När det gäller din offboarding har {{ManagerName}} valt buyout på följande tillgång:### Tillgång:{{AssetName}} ({{AssetId})### Utköpsdatum:{{BuyoutDate}}### Utköpspris:{{BuyoutPrice}} {{Currency}}Det betyder att du får ett löneavdrag på {{BuyoutPrice}} {{Currency}}, och tillgången kommer att äga dig.Viktig! Kom ihåg att radera alla företagsfiler på din sista arbetsdag.", ManagerOnBehalfBuyoutEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Utköp av tillgångar", ManagerOnBehalfBuyoutEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Hej {{FirstName}}!Din chef har valt avkastning på följande tillgång:### Tillgång: {{AssetName}} ({{AssetId})### Datum: {{ReturnDate}}Viktig! Kom ihåg att ta bort alla företagsfiler från din enhet.", ManagerOnBehalfReturnEngTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("Retur av tillgång", ManagerOnBehalfReturnEngSubTemplate.Replace(System.Environment.NewLine, string.Empty));
        }

    }
}
