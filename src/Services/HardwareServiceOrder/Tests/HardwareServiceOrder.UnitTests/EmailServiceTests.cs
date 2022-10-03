using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace HardwareServiceOrder.UnitTests
{
    public class EmailServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IEmailService _emailService;
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;
        public EmailServiceTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>()

        .UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsemail.db").Options)
        {
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var emailOptions = Options.Create(new EmailConfiguration
            {

            });
            _origoConfiguration = new OrigoConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no",
                OrderPath = "/my-business/{0}/hardware-repair/{1}/view"
            };
            var origoOptions = Options.Create(_origoConfiguration);
            var flatDictionary = new FlatDictionary();

            _hardwareServiceOrderContext = new HardwareServiceOrderContext(ContextOptions);

            var mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new EmailProfile());
            }).CreateMapper();

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri != null && x.RequestUri.ToString().Contains("/notification") && x.Method == HttpMethod.Post
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });
            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _emailService = new EmailService(emailOptions, flatDictionary, resourceManger, mapper, origoOptions, _hardwareServiceOrderContext, mockFactory.Object);
        }

        [Fact]
        public void GetEmailTemplate()
        {
            //Arrange
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            //Act
            var orderConfirmationEngTemplate = resourceManger.GetString("OrderConfirmationEmail", CultureInfo.CreateSpecificCulture("EN"));
            var orderConfirmationNoTemplate = resourceManger.GetString("OrderConfirmationEmail", new CultureInfo("no"));
            //Assert
            Assert.NotNull(orderConfirmationEngTemplate);
            Assert.NotNull(orderConfirmationNoTemplate);
        }
        [Fact]
        public void GetEmailTemplate_English()
        {
            //Arrange
            var language = "en";
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!You recently had a repair-order where your asset was discarded. Please contact your manager to get a new asset.", AssetDiscardedEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!You registered a [repair order]({{OrderLink}}) [{{OrderDate}}], but we cannot see having received the asset. Please follow the instructions below so we can help you fix it!Next steps:1. Backup your device2. Factory reset your device3. Remove the SIM card⚠️ if step 1 to 3 is not done properly the repair cannot be completed4. Send the device to the repair provider using the shipping label.5. The repair provider evaluates your device6. You get repair options to choose from (not if warranty)7. The repair provider repairs your asset and sends it back to you[Get shipping label]({{ShippingLabelLink}})PS: The repair provider may contact you directly about the repair.", AssetRepairEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!As you recently had a repair-order, we would like to remind you that any loan-devices needs to be returned. If you don't have any loan-devices, or they are already returned, you may disregard this automated message.", LoanDeviceEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!Your repair order has been canceled. If your asset still needs to be repaired, please register a new repair-order.### **Order details:**Assets: {{AssetName}} ({{AssetId}})  Order date: {{OrderDate}}  Repair type: {{RepairType}}  Fault category: {{FaultCategory}}[View order in Origo]({{OrderLink}})", OrderCancellationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!### **Order details:**Assets: {{AssetName}} ({{AssetId}})  Order date: {{OrderDate}}  Repair type: {{RepairType}}  Fault category: {{FaultCategory}}[View order in Origo]({{OrderLink}})### **Next steps:** 1. Backup your device2. Factory reset your device3. Remove SIM card⚠️ if step 1 to 3 is not done properly the repair cannot be completed4. Send the device to the repair provider using the shipping label5. The repair provider evaluates your device6. You get repair options to choose from (not if warranty)7. The repair provider repairs your device and sends it back to you[Get shipping label]({{ShippingLabelLink}})PS: The repair provider may contact you directly about the repair.### **Loan device:**During the repair period you can get a loan device from your company. Contact [loan device contact info](LoanDeviceContact) if you need a loan device.", OrderConfirmationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{ FirstName }}!### Return details:Asset: {{ AssetName }} {{ AssetId }}Date: {{ OrderDate }}### Next steps:1. Transfer all data from the old asset to the new asset2. Factory reset/delete all data from the old asset3. (if iPhone) Deactivate Find my iPhone4. Remove the SIM card/memory card5. Package and send assetIf step 1 to 4 is not done properly the recycle cannot be completed### How to package and send the asset:Please package your device securely so that it arrives undamaged. You will need to print the shipping label, and use transparent tape to attach it on the outside of the package.PS: The provider may contact you directly about the return.", RemarketingNoPackagingTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{ FirstName }}!### Return details:Asset: {{ AssetName }} {{ AssetId }}Date: {{ OrderDate }}### Would you like to receive packaging?YesAddress: {{ Address }}### Next steps:1. Receive return packaging and shipping label2. Transfer all data from the old asset to the new asset3. Factory reset/delete all data from the old asset4. (if iPhone) Deactivate Find my iPhone5. Remove the SIM card/memory card6. Ship the asset using the return packageIf step 2 to 5 is not done properly the recycle cannot be completedPS: The provider may contact you directly about the return.", RemarketingPackagingTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Norwegian()
        {
            //Arrange
            var language = "no";
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            
            //Act
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hei {{FirstName}}!Du har nylig fått en reparasjonsordre der eiendelen din ble kastet. Ta kontakt med lederen din for å få en ny ressurs.", AssetDiscardedEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!Du registrerte en [reparasjonsordre]({{OrderLink}}) [{{OrderDate}}], men vi kan ikke se å ha mottatt eiendelen. Følg instruksjonene nedenfor slik at vi kan hjelpe deg med å fikse det!Neste skritt:1. Sikkerhetskopier enheten2. Tilbakestill enheten til fabrikkstandard3. Ta ut SIM-kortet⚠️ Hvis trinn 1 til 3 ikke er utført på riktig måte, kan ikke reparasjonen fullføres4. Send enheten til reparasjonsleverandøren ved å bruke fraktetiketten.5. Reparasjonsleverandøren evaluerer enheten din6. Du får reparasjonsalternativer å velge mellom (ikke hvis garanti)7. Reparasjonsleverandøren reparerer eiendelen din og sender den tilbake til deg[Få fraktetikett]({{ShippingLabelLink}})PS: Reparasjonsleverandøren kan kontakte deg direkte angående reparasjonen.", AssetRepairEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!Siden du nylig fikk en reparasjonsordre, vil vi minne deg på at eventuelle låneenheter må returneres. Hvis du ikke har noen låneenheter, eller de allerede er returnert, kan du se bort fra denne automatiske meldingen", LoanDeviceEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!Reparasjonsordren din er kansellert. Hvis eiendelen din fortsatt må repareres, må du registrere en ny reparasjonsordre.### **Ordre detaljer:**Eiendeler: {{AssetName}} ({{AssetId}})Bestillingsdato: {{OrderDate}}Reparasjonstype: {{RepairType}}Feilkategori: {{FaultCategory}}[Se bestillingen i Origo]({{OrderLink}})", OrderCancellationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{FirstName}}!### **Ordre detaljer:**Eiendeler: {{AssetName}} ({{AssetId}})Bestillingsdato: {{OrderDate}}Reparasjonstype: {{RepairType}}Feilkategori: {{FaultCategory}}[Se bestillingen i Origo]({{OrderLink}})### **Neste skritt:**1. Sikkerhetskopier enheten2. Tilbakestill enheten til fabrikkstandard3. Ta ut SIM-kortet⚠️ Hvis trinn 1 til 3 ikke er utført på riktig måte, kan ikke reparasjonen fullføres4. Send enheten til reparasjonsleverandøren ved å bruke fraktetiketten5. Reparasjonsleverandøren evaluerer enheten din6. Du får reparasjonsalternativer å velge mellom (ikke hvis garanti)7. Reparasjonsleverandøren reparerer enheten din og sender den tilbake til deg[Få fraktetikett]({{ShippingLabelLink}})PS: Reparasjonsleverandøren kan kontakte deg direkte angående reparasjonen.### **Låneenhet:**I reparasjonsperioden kan du få låneapparat fra din bedrift. Kontakt [kontaktinformasjon for låneenhet](LoanDeviceContact) hvis du trenger en låneenhet.", OrderConfirmationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hei {{ FirstName }}!### Returdetaljer:Aktiva: {{ AssetName }} {{ AssetId }}Dato: {{ OrderDate }}### Vil du motta emballasje?JaAdresse: {{ Address }}### Neste skritt:1. Motta returemballasje og fraktetikett2. Overfør alle data fra den gamle ressursen til den nye ressursen3. Tilbakestill/slett alle data fra den gamle ressursen4. (hvis iPhone) Deaktiver Finn min iPhone5. Ta ut SIM-kortet/minnekortet6. Send eiendelen med returpakkenHvis trinn 2 til 5 ikke gjøres riktig, kan ikke resirkuleringen fullføresPS: Leverandøren kan kontakte deg direkte angående returen.", RemarketingPackagingTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Swedish()
        {
            //Arrange
            var language = "sv";
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            
            //Act
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hej {{FirstName}}!Du fick nyligen en reparationsorder där din tillgång kasserades. Kontakta din chef för att få en ny tillgång.", AssetDiscardedEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!Du registrerade en [reparationsorder]({{OrderLink}}) [{{OrderDate}}], men vi kan inte se att du har tagit emot tillgången. Följ instruktionerna nedan så att vi kan hjälpa dig att fixa det!Nästa steg:1. Säkerhetskopiera din enhet2. Fabriksåterställ din enhet3. Ta bort SIM-kortet⚠️ Om steg 1 till 3 inte görs korrekt kan reparationen inte slutföras4. Skicka enheten till reparationsleverantören med hjälp av fraktetiketten.5. Reparationsleverantören utvärderar din enhet6. Du får reparationsalternativ att välja mellan (inte om garanti)7. Reparationsleverantören reparerar din tillgång och skickar tillbaka den till dig[Hämta fraktsedel]({{ShippingLabelLink}})PS: Reparationsleverantören kan kontakta dig direkt angående reparationen.", AssetRepairEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!Eftersom du nyligen fick en reparationsorder vill vi påminna dig om att alla låneapparater måste returneras. Om du inte har några låneenheter, eller om de redan är returnerade, kan du bortse från detta automatiska meddelande.", LoanDeviceEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!Din reparationsorder har avbrutits. Om din tillgång fortfarande behöver repareras, registrera en ny reparationsorder.### **Orderdetaljer:**Tillgångar: {{AssetName}} ({{AssetId}})Beställningsdatum: {{OrderDate}}Reparationstyp: {{RepairType}}Felkategori: {{FaultCategory}}[Visa beställning i Origo]({{OrderLink}})", OrderCancellationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!### **Orderdetaljer:**Tillgångar: {{AssetName}} ({{AssetId}})Beställningsdatum: {{OrderDate}}Reparationstyp: {{RepairType}}Felkategori: {{FaultCategory}}[Visa beställning i Origo]({{OrderLink}})### **Nästa steg:**1. Säkerhetskopiera din enhet2. Fabriksåterställ din enhet3. Ta bort SIM-kortet⚠️ Om steg 1 till 3 inte görs korrekt kan reparationen inte slutföras4. Skicka enheten till reparationsleverantören med hjälp av fraktetiketten5. Reparationsleverantören utvärderar din enhet6. Du får reparationsalternativ att välja mellan (inte om garanti)7. Reparationsleverantören reparerar din enhet och skickar tillbaka den till dig[Hämta fraktsedel]({{ShippingLabelLink}})PS: Reparationsleverantören kan kontakta dig direkt angående reparationen.### **Låneenhet:**Under reparationstiden kan du få en låneapparat från ditt företag. Kontakta [kontaktinformation för låneenhet](LoanDeviceContact) om du behöver en låneenhet.", OrderConfirmationEmailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{FirstName}}!### Returinformation:Tillgång: {{ AssetName }} {{ AssetId }}Datum: {{ OrderDate }}### Nästa steg:1. Överför all data från den gamla tillgången till den nya tillgången2. Fabriksåterställning/ta bort all data från den gamla tillgången3. (om iPhone) Inaktivera Hitta min iPhone4. Ta bort SIM-kortet/minneskortet5. Paketera och skicka tillgångOm steg 1 till 4 inte görs korrekt kan återvinningen inte slutföras### Hur man paketerar och skickar tillgången:Vänligen packa din enhet säkert så att den kommer oskadad. Du måste skriva ut fraktetiketten och använda genomskinlig tejp för att fästa den på utsidan av förpackningen.PS: Leverantören kan kontakta dig direkt angående returen.", RemarketingNoPackagingTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hej {{ FirstName }}!### Returinformation:Tillgång: {{ AssetName }} {{ AssetId }}Datum: {{ OrderDate }}### Vill du få förpackning?JaAdress: {{ Address }}### Nästa steg:1. Ta emot returförpackning och fraktetikett2. Överför all data från den gamla tillgången till den nya tillgången3. Fabriksåterställning/ta bort all data från den gamla tillgången4. (om iPhone) Inaktivera Hitta min iPhone5. Ta bort SIM-kortet/minneskortet6. Skicka tillgången med returpaketetOm steg 2 till 5 inte görs korrekt kan återvinningen inte slutförasPS: Leverantören kan kontakta dig direkt angående returen.", RemarketingPackagingTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public async Task SendAssetRepairEmail()
        {
            var emails = await _emailService.SendAssetRepairEmailAsync(DateTime.Today.AddDays(-7), 200);
            Assert.NotNull(emails);
            Assert.Single(emails);

            emails.ForEach(email =>
            {
                Assert.Equal(email.OrderLink, string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId));
            });
        }

        [Fact]
        public async Task SendLoanDeviceEmail()
        {
            var emails = await _emailService.SendLoanDeviceEmailAsync(new List<int> { 200, 300 });
            Assert.NotNull(emails);
            Assert.Equal(2, emails.Count);
            Assert.Equal(2, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsReturnLoanDeviceEmailSent));
        }

        [Fact]
        public async Task SendAssetDiscardEmail()
        {
            var emails = await _emailService.SendOrderDiscardedEmailAsync(200);
            Assert.NotNull(emails);
            Assert.Single(emails);
            Assert.Equal(1, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsOrderDiscardedEmailSent));
        }
    }
}
