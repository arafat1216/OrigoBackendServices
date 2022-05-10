using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Conmodo.ApiModels;
using HardwareServiceOrderServices.Conmodo.Mappings;

namespace HardwareServiceOrderServices.Conmodo
{
    /// <summary>
    ///     The point-of-entry service-class that implements the provider-interfaces for Conmodo.
    /// </summary>
    internal class ProviderServices : IAftermarketProvider, IRepairProvider
    {
        private ApiRequests ApiRequests { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiBaseUrl"> </param>
        /// <param name="apiUsername"> The client's Conmodo Account ID / API Username. </param>
        /// <param name="apiPassword"> </param>
        public ProviderServices(string apiBaseUrl, string apiUsername, string apiPassword)
        {
            ApiRequests = new ApiRequests(apiBaseUrl, apiUsername, apiPassword);
        }


        /// <inheritdoc/>
        public async Task<NewExternalRepairOrderResponseDTO> CreateRepairOrderAsync(NewExternalRepairOrderDTO newRepairOrder, int serviceTypeId, string serviceId)
        {
            string commId = serviceId;
            string? category = new CategoryMapper().ToConmodo(newRepairOrder.AssetInfo.AssetCategoryId);
            StartStatus startStatus = new StartStatusMapper().FromServiceType(serviceTypeId);

            // Create the request objects
            ProductInfo productInfo = new(category, newRepairOrder.AssetInfo.Brand, newRepairOrder.AssetInfo.Model, newRepairOrder.AssetInfo.Imei, newRepairOrder.AssetInfo.SerialNumber, newRepairOrder.AssetInfo.Accessories);
            Delivery deliveryAddress = new(newRepairOrder.DeliveryAddress.Address1, newRepairOrder.DeliveryAddress.Address2, newRepairOrder.DeliveryAddress.PostalCode, newRepairOrder.DeliveryAddress.City);
            Contact serviceRequestOwner = new(newRepairOrder.UserId.ToString(), newRepairOrder.FirstName, newRepairOrder.LastName, newRepairOrder.Email, newRepairOrder.PhoneNumber, deliveryAddress);
            Contact customerHandler = new(newRepairOrder.PartnerId.ToString(), newRepairOrder.PartnerName, newRepairOrder.PartnerOrganizationNumber);
            CreateOrderRequest orderRequest = new(commId, newRepairOrder.OrganizationName, customerHandler, startStatus, newRepairOrder.ErrorDescription, productInfo, newRepairOrder.AssetInfo.PurchaseDate, serviceRequestOwner);

            // Do the request
            var response = await ApiRequests.CreateServiceOrderAsync(orderRequest);

            // Create the return object
            NewExternalRepairOrderResponseDTO repairOrderResponse = new(response.OrderNumber.ToString(), null, response.DirectCustomerLink);
            return repairOrderResponse;
        }


        /// <inheritdoc/>
        public async Task<ExternalRepairOrderDTO> GetRepairOrderAsync(string serviceProviderOrderId1, string? serviceProviderOrderId2)
        {
            throw new NotImplementedException();

            // Make sure the generic string-IDs matches Conmodo's corresponding datatype (to verify the data/format).
            bool id1Parsed = Guid.TryParse(serviceProviderOrderId1, out Guid commId);
            bool id2Parsed = int.TryParse(serviceProviderOrderId2, out int claimNumber);

            if (!id1Parsed)
                throw new ArgumentException("Invalid ID provided");

            var response = await ApiRequests.GetOrderAsync(serviceProviderOrderId1);

            // TODO: Complete implementation
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<ExternalRepairOrderDTO>> GetUpdatedRepairOrdersAsync(DateTimeOffset since)
        {
            throw new NotImplementedException();
        }
    }
}
