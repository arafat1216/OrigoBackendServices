#if DEBUG

using AutoMapper;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A temporary controller used for testing during development.
    ///     All data and methods in this controller is purely for local use, and can be freely removed if needed.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TestController : ControllerBase
    {
        // Dependency injections
        private readonly IProviderFactory _providerFactory;
        private readonly HardwareServiceOrderContext _context;
        private readonly IMapper _mapper;
        private readonly IApiRequesterService _apiRequesterService;
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;


        public TestController(IProviderFactory providerFactory, HardwareServiceOrderContext dbContext, IMapper mapper, IApiRequesterService apiRequesterService, IHardwareServiceOrderService hardwareServiceOrderService, IHardwareServiceOrderRepository hardwareServiceOrderRepository)
        {
            _providerFactory = providerFactory;
            _context = dbContext;
            _mapper = mapper;
            _apiRequesterService = apiRequesterService;
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
        }

        #region Commonly (re-)used methods

        // Please don't delete this one. It's a helper that's used for debugging when we need to fetch the injected caller-ID.
        private object GetResponseObject()
        {
            return new
            {
                AuthenticatedUserId = _apiRequesterService.AuthenticatedUserId
            };
        }

        #endregion

        #region Test-data Population

        /// <summary>
        ///     Add 'service-order' test-data.
        /// </summary>
        /// <remarks>
        ///     Populate the local database with various sample/test-data. 
        ///     This instance will add a single, randomly generated service-order.
        ///     
        ///     <br/><br/>
        ///     NB: This should only be performed on a local, unpopulated database! Your local database should be
        ///     dropped and re-created before you run this to avoid problems/conflicts.
        /// </remarks>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpPost("test-data/populate/service-order")]
        [Tags("Populate test-data")]
        public async Task<IActionResult> PopulateLocalTestData_NewOrder_Async([FromQuery] Guid? customerId = null, [FromQuery] Guid? userId = null, [FromQuery] Guid? assetLifecyleId = null)
        {
            customerId ??= Guid.NewGuid();
            userId ??= Guid.NewGuid();
            assetLifecyleId ??= Guid.NewGuid();
            Random random = new(); // NOTE: The random max value is exclusive, not inclusive!

            HardwareServiceOrderServices.Models.DeliveryAddress deliveryAddress = new(
                (RecipientTypeEnum)random.Next(1, 3),
                $"{GenerateName(random.Next(4, 12))} {GenerateName(random.Next(4, 13))}",
                $"{GenerateName(random.Next(4, 13))} Street",
                null,
                $"{random.Next(1000, 10000)}",
                $"{GenerateName(random.Next(5, 12))}",
                $"NO"
            );

            HardwareServiceOrderServices.Models.ContactDetails owner = new(
                userId.Value,
                (random.Next(0, 101) >= 80) ? $"{GenerateName(random.Next(4, 12))} {GenerateName(random.Next(4, 12))}" : GenerateName(random.Next(4, 12)), // 20% chance for double first name
                (random.Next(0, 101) >= 80) ? $"{GenerateName(random.Next(4, 12))} {GenerateName(random.Next(4, 12))}" : GenerateName(random.Next(4, 12)), // 20% chance for double last name
                $"{GenerateName(random.Next(4, 12))}@{GenerateName(random.Next(4, 12))}.{GenerateName(random.Next(2, 3))}".ToLower(),
                (random.Next(0, 101) >= 80) ? $"+47{random.Next(80000000, 99999999)}" : null // 20% chance to add a phone-number
            );

            List<HardwareServiceOrderServices.Models.ServiceEvent> serviceEvents = new() { };
            for (int i = 0; i < random.Next(0, 8); i++)
            {
                serviceEvents.Add(new HardwareServiceOrderServices.Models.ServiceEvent()
                {
                    ServiceStatusId = random.Next(1, 17),
                    Timestamp = GenerateDateTimeOffset()
                });
            }

            HashSet<int> includedServiceOrderAddonIds = new() { };
            if (random.Next(0, 2) == 1)
                includedServiceOrderAddonIds.Add(1);

            HardwareServiceOrderServices.Models.AssetInfo asset;
            if (random.Next(0, 2) == 1)
                asset = new("Samsung", $"Galaxy S{random.Next(10, 23)}", new HashSet<string>() { random.NextInt64(888888888888888, 999999999999999).ToString() }, null, GenerateDateOnly(), null);
            else
                asset = new("Apple", $"iPhone {random.Next(6, 16)}", new HashSet<string>() { random.NextInt64(888888888888888, 999999999999999).ToString() }, null, GenerateDateOnly(), null);

            HardwareServiceOrderServices.Models.HardwareServiceOrder serviceOrder = new(
                customerId.Value,
                assetLifecyleId.Value,
                1,
                asset,
                $"{GenerateSentence(random.Next(0, 20))}",
                owner,
                deliveryAddress,
                1,
                random.Next(1, 17),
                1,
                includedServiceOrderAddonIds,
                Guid.NewGuid().ToString(),
                null,
                $"https://example.com/user?{GenerateName(9)}&password={GenerateName(9)}",
                serviceEvents
            );

            _context.HardwareServiceOrders.Add(serviceOrder);
            await _context.SaveChangesAsync();

            return Ok(serviceOrder);
        }


        // Generates random sentences. Only to be used when adding random test-data.
        private static string GenerateSentence(int length)
        {
            Random random = new();
            StringBuilder sb = new();

            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                    sb.Append(' ');

                sb.Append(GenerateName(random.Next(1, 10)));
            }

            return sb.ToString();
        }

        // Generates a random DateTimeOffset. Only to be used when adding random test-data.
        private static DateTimeOffset GenerateDateTimeOffset()
        {
            Random random = new();

            DateTimeOffset result = DateTimeOffset.Now;
            result.AddYears(-random.Next(0, 4));
            result.AddMonths(-random.Next(0, 13));
            result.AddDays(-random.Next(0, 32));
            result.AddHours(-random.Next(0, 25));
            result.AddMinutes(-random.Next(0, 61));

            return result;
        }

        // Generates a random DateOnly. Only to be used when adding random test-data.
        private static DateOnly GenerateDateOnly()
        {
            Random random = new();

            DateOnly result = DateOnly.FromDateTime(DateTime.Now);
            result.AddYears(-random.Next(0, 4));
            result.AddMonths(-random.Next(0, 13));
            result.AddDays(-random.Next(0, 32));

            return result;
        }

        // Generates random names/words. Only to be used when adding random test-data.
        private static string GenerateName(int length)
        {
            Random random = new();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[random.Next(consonants.Length)].ToUpper();
            Name += vowels[random.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < length)
            {
                Name += consonants[random.Next(consonants.Length)];
                b++;
                Name += vowels[random.Next(vowels.Length)];
                b++;
            }

            return Name;
        }

        #endregion


        /// <summary>
        ///     Lists every <c>CustomerServiceProvider</c> that exist in the database.
        /// </summary>
        /// <remarks>
        ///     Lists every <c>CustomerServiceProvider</c> that exist in the database. 
        ///     If requested, it's re-mapped to <c>CustomerServiceProviderDto</c>.
        /// </remarks>
        /// <param name="dto"> If <c><see langword="true"/></c>, it returns the remapped DTO entity. <br/>
        ///     If <c><see langword="false"/></c> it returns the Entity Framework entity. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("customer-service-provider")]
        public async Task<ActionResult> GetAllCustomerServiceProvidersAsync([FromQuery] bool dto = false)
        {
            List<HardwareServiceOrderServices.Models.CustomerServiceProvider> results = await _context.CustomerServiceProviders
                                                                                                      .Include(e => e.ApiCredentials)
                                                                                                      .ToListAsync();

            if (dto)
            {
                IEnumerable<CustomerServiceProviderDto> mapped = _mapper.Map<IEnumerable<CustomerServiceProviderDto>>(results);
                return Ok(mapped);
            }
            else
            {
                return Accepted(results);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"> The key which is used to encrypt the provided text. Here we are using a Guid as key</param>
        /// <param name="textToEncrypt"> The text that needs to encrypt </param>
        /// <returns></returns>
        [HttpGet("{key:Guid}/encrypt")]
        public string Encrypt(Guid key, [FromQuery] string textToEncrypt)
        {
            var encryptedText = _hardwareServiceOrderRepository.Encrypt(textToEncrypt, key.ToString());
            return encryptedText;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"> The key which is used to encrypt the provided text. Here we are using a Guid as key</param>
        /// <param name="encryptedText"> The encrypted text that needs to decrypt </param>
        /// <returns></returns>
        [HttpGet("{key:Guid}/decrypt")]
        public string Decrypt(Guid key, [FromQuery] string encryptedText)
        {
            var decryptedText = _hardwareServiceOrderRepository.Decrypt(encryptedText, key.ToString());
            return decryptedText;
        }

    }
}

#endif
