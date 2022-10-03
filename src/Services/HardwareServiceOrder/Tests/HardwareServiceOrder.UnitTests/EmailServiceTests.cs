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
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));

            var orderConfirmationEngTemplate = resourceManger.GetString("OrderConfirmationEmail", CultureInfo.CreateSpecificCulture("EN"));
            Assert.NotNull(orderConfirmationEngTemplate);
            var orderConfirmationNoTemplate = resourceManger.GetString("OrderConfirmationEmail", new CultureInfo("no"));
            Assert.NotNull(orderConfirmationNoTemplate);
        }
        [Fact]
        public void GetEmailTemplate_English()
        {
            var language = "en";

            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nYou recently had a repair-order where your asset was discarded. Please contact your manager to get a new asset.", AssetDiscardedEmailTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nYou registered a [repair order]({{OrderLink}}) [{{OrderDate}}], but we cannot see having received the asset. Please follow the instructions below so we can help you fix it!\r\n\r\nNext steps:\r\n\r\n1. Backup your device\r\n2. Factory reset your device\r\n3. Remove the SIM card\r\n\r\n⚠️ if step 1 to 3 is not done properly the repair cannot be completed\r\n\r\n4. Send the device to the repair provider using the shipping label.\r\n5. The repair provider evaluates your device\r\n6. You get repair options to choose from (not if warranty)\r\n7. The repair provider repairs your asset and sends it back to you\r\n\r\n[Get shipping label]({{ShippingLabelLink}})\r\n\r\nPS: The repair provider may contact you directly about the repair.", AssetRepairEmailTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nAs you recently had a repair-order, we would like to remind you that any loan-devices needs to be returned. If you don't have any loan-devices, or they are already returned, you may disregard this automated message.", LoanDeviceEmailTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nYour repair order has been canceled. If your asset still needs to be repaired, please register a new repair-order.\r\n\r\n\r\n### **Order details:**\r\n\r\nAssets: {{AssetName}} ({{AssetId}})  \r\nOrder date: {{OrderDate}}  \r\nRepair type: {{RepairType}}  \r\nFault category: {{FaultCategory}}\r\n\r\n[View order in Origo]({{OrderLink}})", OrderCancellationEmailTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\n### **Order details:**\r\n\r\nAssets: {{AssetName}} ({{AssetId}})  \r\nOrder date: {{OrderDate}}  \r\nRepair type: {{RepairType}}  \r\nFault category: {{FaultCategory}}\r\n\r\n[View order in Origo]({{OrderLink}})\r\n\r\n### **Next steps:** \r\n1. Backup your device\r\n2. Factory reset your device\r\n3. Remove SIM card\r\n\r\n⚠️ if step 1 to 3 is not done properly the repair cannot be completed\r\n\r\n4. Send the device to the repair provider using the shipping label\r\n5. The repair provider evaluates your device\r\n6. You get repair options to choose from (not if warranty)\r\n7. The repair provider repairs your device and sends it back to you\r\n\r\n[Get shipping label]({{ShippingLabelLink}})\r\n\r\nPS: The repair provider may contact you directly about the repair.\r\n\r\n\r\n### **Loan device:**\r\n\r\nDuring the repair period you can get a loan device from your company. Contact [loan device contact info](LoanDeviceContact) if you need a loan device.", OrderConfirmationEmailTemplate);
            Assert.Equal("### Hello {{ FirstName }}!\r\n\r\n\r\n### Return details:\r\nAsset: {{ AssetName }} {{ AssetId }}\r\nDate: {{ OrderDate }}\r\n\r\n### Next steps:\r\n1. Transfer all data from the old asset to the new asset\r\n2. Factory reset/delete all data from the old asset\r\n3. (if iPhone) Deactivate Find my iPhone\r\n4. Remove the SIM card/memory card\r\n5. Package and send asset\r\n\r\nIf step 1 to 4 is not done properly the recycle cannot be completed\r\n\r\n### How to package and send the asset:\r\nPlease package your device securely so that it arrives undamaged. You will need to print the shipping label, and use transparent tape to attach it on the outside of the package.\r\n\r\nPS: The provider may contact you directly about the return.", RemarketingNoPackagingTemplate);
            Assert.Equal("### Hello {{ FirstName }}!\r\n\r\n\r\n### Return details:\r\nAsset: {{ AssetName }} {{ AssetId }}\r\nDate: {{ OrderDate }}\r\n\r\n### Would you like to receive packaging?\r\nYes\r\nAddress: {{ Address }}\r\n\r\n### Next steps:\r\n1. Receive return packaging and shipping label\r\n2. Transfer all data from the old asset to the new asset\r\n3. Factory reset/delete all data from the old asset\r\n4. (if iPhone) Deactivate Find my iPhone\r\n5. Remove the SIM card/memory card\r\n6. Ship the asset using the return package\r\n\r\nIf step 2 to 5 is not done properly the recycle cannot be completed\r\n\r\nPS: The provider may contact you directly about the return.", RemarketingPackagingTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Norwegian()
        {
            var language = "no";

            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nDu har nylig fått en reparasjonsordre der eiendelen din ble kastet. Ta kontakt med lederen din for å få en ny ressurs.", AssetDiscardedEmailTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nDu registrerte en [reparasjonsordre]({{OrderLink}}) [{{OrderDate}}], men vi kan ikke se å ha mottatt eiendelen. Følg instruksjonene nedenfor slik at vi kan hjelpe deg med å fikse det!\r\n\r\nNeste skritt:\r\n\r\n1. Sikkerhetskopier enheten\r\n2. Tilbakestill enheten til fabrikkstandard\r\n3. Ta ut SIM-kortet\r\n\r\n⚠️ Hvis trinn 1 til 3 ikke er utført på riktig måte, kan ikke reparasjonen fullføres\r\n\r\n4. Send enheten til reparasjonsleverandøren ved å bruke fraktetiketten.\r\n5. Reparasjonsleverandøren evaluerer enheten din\r\n6. Du får reparasjonsalternativer å velge mellom (ikke hvis garanti)\r\n7. Reparasjonsleverandøren reparerer eiendelen din og sender den tilbake til deg\r\n\r\n[Få fraktetikett]({{ShippingLabelLink}})\r\n\r\nPS: Reparasjonsleverandøren kan kontakte deg direkte angående reparasjonen.", AssetRepairEmailTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nSiden du nylig fikk en reparasjonsordre, vil vi minne deg på at eventuelle låneenheter må returneres. Hvis du ikke har noen låneenheter, eller de allerede er returnert, kan du se bort fra denne automatiske meldingen", LoanDeviceEmailTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\nReparasjonsordren din er kansellert. Hvis eiendelen din fortsatt må repareres, må du registrere en ny reparasjonsordre.\r\n\r\n\r\n### **Ordre detaljer:**\r\n\r\nEiendeler: {{AssetName}} ({{AssetId}})\r\nBestillingsdato: {{OrderDate}}\r\nReparasjonstype: {{RepairType}}\r\nFeilkategori: {{FaultCategory}}\r\n\r\n[Se bestillingen i Origo]({{OrderLink}})", OrderCancellationEmailTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\n### **Ordre detaljer:**\r\n\r\nEiendeler: {{AssetName}} ({{AssetId}})\r\nBestillingsdato: {{OrderDate}}\r\nReparasjonstype: {{RepairType}}\r\nFeilkategori: {{FaultCategory}}\r\n\r\n[Se bestillingen i Origo]({{OrderLink}})\r\n\r\n### **Neste skritt:**\r\n1. Sikkerhetskopier enheten\r\n2. Tilbakestill enheten til fabrikkstandard\r\n3. Ta ut SIM-kortet\r\n\r\n⚠️ Hvis trinn 1 til 3 ikke er utført på riktig måte, kan ikke reparasjonen fullføres\r\n\r\n4. Send enheten til reparasjonsleverandøren ved å bruke fraktetiketten\r\n5. Reparasjonsleverandøren evaluerer enheten din\r\n6. Du får reparasjonsalternativer å velge mellom (ikke hvis garanti)\r\n7. Reparasjonsleverandøren reparerer enheten din og sender den tilbake til deg\r\n\r\n[Få fraktetikett]({{ShippingLabelLink}})\r\n\r\nPS: Reparasjonsleverandøren kan kontakte deg direkte angående reparasjonen.\r\n\r\n\r\n### **Låneenhet:**\r\n\r\nI reparasjonsperioden kan du få låneapparat fra din bedrift. Kontakt [kontaktinformasjon for låneenhet](LoanDeviceContact) hvis du trenger en låneenhet.", OrderConfirmationEmailTemplate);
            Assert.Equal("### Hei {{FirstName}}!\r\n\r\n\r\n### Returdetaljer:\r\nAktiva: {{ AssetName }} {{ AssetId }}\r\nDato: {{ OrderDate }}\r\n\r\n### Neste skritt:\r\n1. Overfør alle data fra den gamle ressursen til den nye ressursen\r\n2. Tilbakestill/slett alle data fra den gamle ressursen\r\n3. (hvis iPhone) Deaktiver Finn min iPhone\r\n4. Ta ut SIM-kortet/minnekortet\r\n5. Pakk og send eiendel\r\n\r\nHvis trinn 1 til 4 ikke gjøres riktig, kan ikke resirkuleringen fullføres\r\n\r\n### Hvordan pakke og sende eiendelen:\r\nPakk enheten på en sikker måte slik at den kommer uskadet. Du må skrive ut fraktetiketten og bruke gjennomsiktig tape for å feste den på utsiden av pakken.\r\n\r\nPS: Leverandøren kan kontakte deg direkte angående returen.", RemarketingNoPackagingTemplate);
            Assert.Equal("### Hei {{ FirstName }}!\r\n\r\n\r\n### Returdetaljer:\r\nAktiva: {{ AssetName }} {{ AssetId }}\r\nDato: {{ OrderDate }}\r\n\r\n### Vil du motta emballasje?\r\nJa\r\nAdresse: {{ Address }}\r\n\r\n### Neste skritt:\r\n1. Motta returemballasje og fraktetikett\r\n2. Overfør alle data fra den gamle ressursen til den nye ressursen\r\n3. Tilbakestill/slett alle data fra den gamle ressursen\r\n4. (hvis iPhone) Deaktiver Finn min iPhone\r\n5. Ta ut SIM-kortet/minnekortet\r\n6. Send eiendelen med returpakken\r\n\r\nHvis trinn 2 til 5 ikke gjøres riktig, kan ikke resirkuleringen fullføres\r\n\r\nPS: Leverandøren kan kontakte deg direkte angående returen.", RemarketingPackagingTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Swedish()
        {
            var language = "sv";

            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var AssetDiscardedEmailTemplate = resourceManger.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var AssetRepairEmailTemplate = resourceManger.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var LoanDeviceEmailTemplate = resourceManger.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderCancellationEmailTemplate = resourceManger.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var OrderConfirmationEmailTemplate = resourceManger.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingNoPackagingTemplate = resourceManger.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));
            var RemarketingPackagingTemplate = resourceManger.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(language));


            Assert.NotNull(AssetDiscardedEmailTemplate);
            Assert.NotNull(AssetRepairEmailTemplate);
            Assert.NotNull(OrderCancellationEmailTemplate);
            Assert.NotNull(LoanDeviceEmailTemplate);
            Assert.NotNull(OrderConfirmationEmailTemplate);
            Assert.NotNull(RemarketingNoPackagingTemplate);
            Assert.NotNull(RemarketingPackagingTemplate);

            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nDu fick nyligen en reparationsorder där din tillgång kasserades. Kontakta din chef för att få en ny tillgång.", AssetDiscardedEmailTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nDu registrerade en [reparationsorder]({{OrderLink}}) [{{OrderDate}}], men vi kan inte se att du har tagit emot tillgången. Följ instruktionerna nedan så att vi kan hjälpa dig att fixa det!\r\n\r\nNästa steg:\r\n\r\n1. Säkerhetskopiera din enhet\r\n2. Fabriksåterställ din enhet\r\n3. Ta bort SIM-kortet\r\n\r\n⚠️ Om steg 1 till 3 inte görs korrekt kan reparationen inte slutföras\r\n\r\n4. Skicka enheten till reparationsleverantören med hjälp av fraktetiketten.\r\n5. Reparationsleverantören utvärderar din enhet\r\n6. Du får reparationsalternativ att välja mellan (inte om garanti)\r\n7. Reparationsleverantören reparerar din tillgång och skickar tillbaka den till dig\r\n\r\n[Hämta fraktsedel]({{ShippingLabelLink}})\r\n\r\nPS: Reparationsleverantören kan kontakta dig direkt angående reparationen.", AssetRepairEmailTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nEftersom du nyligen fick en reparationsorder vill vi påminna dig om att alla låneapparater måste returneras. Om du inte har några låneenheter, eller om de redan är returnerade, kan du bortse från detta automatiska meddelande.", LoanDeviceEmailTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nDin reparationsorder har avbrutits. Om din tillgång fortfarande behöver repareras, registrera en ny reparationsorder.\r\n\r\n\r\n### **Orderdetaljer:**\r\n\r\nTillgångar: {{AssetName}} ({{AssetId}})\r\nBeställningsdatum: {{OrderDate}}\r\nReparationstyp: {{RepairType}}\r\nFelkategori: {{FaultCategory}}\r\n\r\n[Visa beställning i Origo]({{OrderLink}})", OrderCancellationEmailTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\n### **Orderdetaljer:**\r\n\r\nTillgångar: {{AssetName}} ({{AssetId}})\r\nBeställningsdatum: {{OrderDate}}\r\nReparationstyp: {{RepairType}}\r\nFelkategori: {{FaultCategory}}\r\n\r\n[Visa beställning i Origo]({{OrderLink}})\r\n\r\n### **Nästa steg:**\r\n1. Säkerhetskopiera din enhet\r\n2. Fabriksåterställ din enhet\r\n3. Ta bort SIM-kortet\r\n\r\n⚠️ Om steg 1 till 3 inte görs korrekt kan reparationen inte slutföras\r\n\r\n4. Skicka enheten till reparationsleverantören med hjälp av fraktetiketten\r\n5. Reparationsleverantören utvärderar din enhet\r\n6. Du får reparationsalternativ att välja mellan (inte om garanti)\r\n7. Reparationsleverantören reparerar din enhet och skickar tillbaka den till dig\r\n\r\n[Hämta fraktsedel]({{ShippingLabelLink}})\r\n\r\nPS: Reparationsleverantören kan kontakta dig direkt angående reparationen.\r\n\r\n\r\n### **Låneenhet:**\r\n\r\nUnder reparationstiden kan du få en låneapparat från ditt företag. Kontakta [kontaktinformation för låneenhet](LoanDeviceContact) om du behöver en låneenhet.", OrderConfirmationEmailTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\n\r\n### Returinformation:\r\nTillgång: {{ AssetName }} {{ AssetId }}\r\nDatum: {{ OrderDate }}\r\n\r\n### Nästa steg:\r\n1. Överför all data från den gamla tillgången till den nya tillgången\r\n2. Fabriksåterställning/ta bort all data från den gamla tillgången\r\n3. (om iPhone) Inaktivera Hitta min iPhone\r\n4. Ta bort SIM-kortet/minneskortet\r\n5. Paketera och skicka tillgång\r\n\r\nOm steg 1 till 4 inte görs korrekt kan återvinningen inte slutföras\r\n\r\n### Hur man paketerar och skickar tillgången:\r\nVänligen packa din enhet säkert så att den kommer oskadad. Du måste skriva ut fraktetiketten och använda genomskinlig tejp för att fästa den på utsidan av förpackningen.\r\n\r\nPS: Leverantören kan kontakta dig direkt angående returen.", RemarketingNoPackagingTemplate);
            Assert.Equal("### Hej {{ FirstName }}!\r\n\r\n\r\n### Returinformation:\r\nTillgång: {{ AssetName }} {{ AssetId }}\r\nDatum: {{ OrderDate }}\r\n\r\n### Vill du få förpackning?\r\nJa\r\nAdress: {{ Address }}\r\n\r\n### Nästa steg:\r\n1. Ta emot returförpackning och fraktetikett\r\n2. Överför all data från den gamla tillgången till den nya tillgången\r\n3. Fabriksåterställning/ta bort all data från den gamla tillgången\r\n4. (om iPhone) Inaktivera Hitta min iPhone\r\n5. Ta bort SIM-kortet/minneskortet\r\n6. Skicka tillgången med returpaketet\r\n\r\nOm steg 2 till 5 inte görs korrekt kan återvinningen inte slutföras\r\n\r\nPS: Leverantören kan kontakta dig direkt angående returen.", RemarketingPackagingTemplate);
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
