using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Conmodo.ApiModels;
using HardwareServiceOrderServices.Conmodo.Mappings;
using System.Text.RegularExpressions;

namespace HardwareServiceOrderServices.Conmodo
{
    /// <summary>
    ///     The point-of-entry service-class that implements the provider-interfaces for Conmodo.
    /// </summary>
    public class ConmodoProviderServices : IRepairProvider
    {
        private IApiRequests ApiRequests { get; }

        /// <summary>
        ///     Initializes a new instance of <see cref="IRepairProvider"/> and <see cref="IAftermarketProvider"/> using Conmodo as the service-provider.
        /// </summary>
        /// <param name="apiBaseUrl"> The URL base path to Conmodo's API, ending with a slash. E.g. "<c>https://service-test.conmodo.com/api/v1/</c>". </param>
        /// <param name="apiUsername"> The customer's account-ID / API username. </param>
        /// <param name="apiPassword"> Our API password. </param>
        public ConmodoProviderServices(string apiBaseUrl, string apiUsername, string apiPassword)
        {
            ApiRequests = new ApiRequests(apiBaseUrl, apiUsername, apiPassword);
        }


        /// <summary>
        ///     Unit-test constructor
        /// </summary>
        internal ConmodoProviderServices(IApiRequests apiRequests)
        {
            ApiRequests = apiRequests;
        }


        /// <inheritdoc/>
        public async Task<NewExternalServiceOrderResponseDTO> CreateRepairOrderAsync(NewExternalRepairOrderDTO newRepairOrder, int serviceTypeId, string serviceOrderId)
        {
            if (string.IsNullOrEmpty(newRepairOrder.AssetInfo.Brand))
                throw new ArgumentException("The asset's brand name is missing.", nameof(newRepairOrder));
            if (string.IsNullOrEmpty(newRepairOrder.AssetInfo.Model))
                throw new ArgumentException("The asset's model name is missing.", nameof(newRepairOrder));

            // TODO: This list-implementation is a quick-fix. Once we add in support for pre-swap or Swedish customers, this needs to be replaced.
            // Registers extra services
            HashSet<int> extraServices = new()
            {
                (int)ExtraServicesEnum.SentInByCustomerOrUser_NO,
                (int)ExtraServicesEnum.ReturnToCustomerOrUser_NO
            };

            string? category = new CategoryMapper().ToConmodo(newRepairOrder.AssetInfo.AssetCategoryId);
            StartStatus startStatus = new StartStatusMapper().FromServiceType(serviceTypeId);
            ProductInfo productInfo = new(category, newRepairOrder.AssetInfo.Brand, newRepairOrder.AssetInfo.Model, newRepairOrder.AssetInfo.Imei, newRepairOrder.AssetInfo.SerialNumber, newRepairOrder.AssetInfo.Accessories);
            Delivery deliveryAddress = new(newRepairOrder.DeliveryAddress.Address1, newRepairOrder.DeliveryAddress.Address2, newRepairOrder.DeliveryAddress.PostalCode, newRepairOrder.DeliveryAddress.City);
            Contact customerHandler = new(newRepairOrder.PartnerId.ToString(), newRepairOrder.PartnerName, newRepairOrder.PartnerOrganizationNumber);
            Contact serviceRequestOwner;
            
            // Return to a company
            if (newRepairOrder.DeliveryAddress.RecipientType == Models.RecipientTypeEnum.Organization)
                serviceRequestOwner = new(newRepairOrder.UserId.ToString(), newRepairOrder.FirstName, newRepairOrder.LastName, newRepairOrder.OrganizationName, newRepairOrder.OrganizationNumber, newRepairOrder.Email, newRepairOrder.PhoneNumber, deliveryAddress, newRepairOrder.DeliveryAddress.Country);
            // Return to a user
            else if (newRepairOrder.DeliveryAddress.RecipientType == Models.RecipientTypeEnum.Personal)
                serviceRequestOwner = new(newRepairOrder.UserId.ToString(), newRepairOrder.FirstName, newRepairOrder.LastName, newRepairOrder.Email, newRepairOrder.PhoneNumber, deliveryAddress, newRepairOrder.DeliveryAddress.Country);
            // Unsupported value
            else
                throw new ArgumentException("An unsupported recipient type was used for the return address.");
            
            CreateOrderRequest orderRequest = new(serviceOrderId, $"Origo - {newRepairOrder.OrganizationName}", customerHandler, startStatus, newRepairOrder.ErrorDescription, productInfo, newRepairOrder.AssetInfo.PurchaseDate, serviceRequestOwner, extraServices);

            // Do the request
            var response = await ApiRequests.CreateServiceOrderAsync(orderRequest);

            // Create the return object
            NewExternalServiceOrderResponseDTO repairOrderResponse = new(serviceOrderId, response.OrderNumber.ToString(), response.DirectCustomerLink);
            return repairOrderResponse;
        }



        /// <inheritdoc/>
        /// <remarks>
        ///     Handles the parameter validation.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "It already does this...")]
        public async Task<ExternalRepairOrderDTO> GetRepairOrderAsync(string serviceProviderOrderId1, string? serviceProviderOrderId2)
        {
            // Make sure the generic string-IDs matches Conmodo's corresponding datatype (to verify the data/format).
            bool id1Validated = Guid.TryParse(serviceProviderOrderId1, out Guid commId);
            bool id2Validated = int.TryParse(serviceProviderOrderId2, out int claimNumber);

            // If ID-1 failed to validate using the newer Guid format, see if it matches the old - deprecated - NOLF format
            if (!id1Validated)
            {
                // Regex explanation: One required group followed by an optional group.
                // The first group must always start with "NOLF" followed by at least one digit.
                // The second optional group starts with a "-" followed by at least one digit.
                // Examples:
                // NOLF<one or more numbers>
                // NOLF<one or more numbers>-<one or more numbers>
                id1Validated = Regex.IsMatch(serviceProviderOrderId1.ToUpper(), "^(NOLF+\\d{1,}){1,1}([-]{1}\\d{1,}){0,1}$");
            }

            if (!id1Validated)
                throw new ArgumentException("Invalid ID provided.", nameof(serviceProviderOrderId1));
            if (serviceProviderOrderId2 is not null && !id2Validated)
                throw new ArgumentException("Invalid ID provided.", nameof(serviceProviderOrderId2));

            if (id2Validated)
                return await GetRepairOrderPostValidationAsync(serviceProviderOrderId1, claimNumber);
            else
                return await GetRepairOrderPostValidationAsync(serviceProviderOrderId1, null);
        }


        /// <inheritdoc cref="GetRepairOrderAsync(string, string?)"/>
        /// <remarks>
        ///     Handles the <see langword="async"/> portion of <see cref="GetRepairOrderAsync(string, string?)"/> once the input has been validated.
        /// </remarks>
        /// <param name="commId"> Our custom identifier that was provided to Conmodo when we created the service-order. </param>
        /// <param name="orderNo"> Conmodo's order-number. In some parts of Conmodo's documentation this is referred to as <c>claimNumber</c>. </param>
        private async Task<ExternalRepairOrderDTO> GetRepairOrderPostValidationAsync(string commId, int? orderNo)
        {
            OrderResponse? response = await ApiRequests.GetOrderAsync(commId);
            AssetInfoDTO? deliveredAsset = null;
            AssetInfoDTO? returnedAsset = null;

            // Set the orderNo value if it's missing.
            orderNo ??= response.DeltaOrderNumber;

            // If possible, create the "delivered asset" object based on whatever information we have available
            if (response.ProductInfoIn is not null)
            {
                string? imei = null;

                // We have two potential inputs for the IMEI, so lets pick the first one with an actual value.
                if (!string.IsNullOrEmpty(response.ProductInfoIn.Imei))
                    imei = response.ProductInfoIn.Imei;
                else if (!string.IsNullOrEmpty(response.RegisteredImeiIn))
                    imei = response.RegisteredImeiIn;

                deliveredAsset = new(response.ProductInfoIn.Brand, response.ProductInfoIn.Model, imei, response.ProductInfoIn.Serial, response.ProductInfoIn.Accessories);
            }
            else if (!string.IsNullOrEmpty(response.RegisteredImeiIn))
            {
                deliveredAsset = new(null, null, response.RegisteredImeiIn, null);
            }

            // If possible, create the "returned asset" object based on whatever information we have available
            if (response.ProductInfoOut is not null)
            {
                returnedAsset = new(response.ProductInfoOut.Brand, response.ProductInfoOut.Model, response.ProductInfoOut.Imei, response.ProductInfoOut.Serial, response.ProductInfoOut.Accessories);
            }

            EventMapper eventMapper = new();
            bool? isAssetReplaced = CheckForAssetReplacement(deliveredAsset, returnedAsset, response.Events);
            var externalEventList = eventMapper.FromConmodo(response.Events, isAssetReplaced);

            ExternalRepairOrderDTO repairOrder = new(commId, orderNo.ToString(), externalEventList, deliveredAsset, returnedAsset, isAssetReplaced);
            return repairOrder;
        }

        /// <summary>
        ///     Attempts to detect if an asset has been replaced.
        /// </summary>
        /// <param name="deliveredAsset"> If available, details about the delivered asset. </param>
        /// <param name="returnedAsset"> If available, details about the returned asset. </param>
        /// <param name="conmodoEvents"> The list of events that is used when checking if the order is completed. </param>
        /// <returns> Returns <see langword="null"/> if a service is in-progress, or we can't determine of a replacement has taken place.
        ///     Otherwise it returns <see langword="true"/> when a replacement was detected, and <see langword="false"/> when the service was completed
        ///     without a replacement device. </returns>
        private bool? CheckForAssetReplacement(AssetInfoDTO? deliveredAsset, AssetInfoDTO? returnedAsset, IEnumerable<Event> conmodoEvents)
        {
            // If the details for the delivered asset is missing we return the unknown (null) value
            if (deliveredAsset is null || string.IsNullOrEmpty(deliveredAsset.Imei))
                return null;

            EventMapper eventMapper = new();
            bool anyCompletedEventExist = eventMapper.ContainsAnyCompletedEvents(conmodoEvents);

            // No completed events exist, so we assume it's still in progress and that we don't yet know if any replacements have/will take place
            if (!anyCompletedEventExist)
                return null;

            // ONLY CONDITIONS WITH A IMPLICIT "anyCompletedEventExist == true" CONDITION SHOULD BE PLACED BELOW THIS POINT

            // We have a completed event, but haven't gotten any details about a new device. We therefore assume no replacements have taken place
            if (returnedAsset is null || string.IsNullOrEmpty(returnedAsset.Imei))
                return false;

            // The IMEI's matches. No replacement has taken place.
            if (string.Equals(deliveredAsset.Imei, returnedAsset.Imei))
                return false;
            else
                return true;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<ExternalRepairOrderDTO>> GetUpdatedRepairOrdersAsync(DateTimeOffset since)
        {
            var ordersWithUpdates = await ApiRequests.GetUpdatedOrdersAsync(since);

            if (ordersWithUpdates is null || ordersWithUpdates.Order is null || !ordersWithUpdates.Order.Any())
                return new List<ExternalRepairOrderDTO>();

            // A list that will contain all async tasks added in the foreach loop
            List<Task<ExternalRepairOrderDTO>> listOfTasks = new();

            // Add the task itself to the list. DO NOT await anything inside the loop, as this defeats the purpose of starting all external requests in parallel!
            foreach (var updatedOrder in ordersWithUpdates.Order)
            {
                // This should really never happen, but is added for safety since Conmodo's documentation has the property marked as nullable.
                if (string.IsNullOrEmpty(updatedOrder.CommId))
                    continue;

                listOfTasks.Add(GetRepairOrderAsync(updatedOrder.CommId, updatedOrder.OrderNo?.ToString()));
            }

            // Once all the async fetches has been completed, return the result.
            return await Task.WhenAll(listOfTasks);
        }


        /// <summary>
        ///     Trigger a simple request to Conmodo's test-call endpoint.
        /// </summary>
        /// <returns> If the endpoint was reached, it returns the string "<c>Ok!</c>". </returns>
        public async Task<string> TestCall()
        {
            return await ApiRequests.TestAsync();
        }
    }
}
