using HardwareServiceOrderServices.Conmodo.ApiModels;

namespace HardwareServiceOrderServices.Conmodo
{
    /// <summary>
    ///     Defines the API requests supported in Conmodo's API
    /// </summary>
    internal interface IApiRequests
    {
        /// <summary>
        ///     Perform a simple test-call to the API.
        /// </summary>
        /// <returns> The retrieved value. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code from Conmodo, or if we receive an invalid response object. </exception>
        public Task<string> TestAsync();

        /// <summary>
        ///     Retrieves the status for the requested order.
        /// </summary>
        /// <param name="commId"> Conmodo's service ID. </param>
        /// <returns> An object containing the details for the requested service-order. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code from Conmodo, or if we receive an invalid response object. </exception>
        public Task<OrderResponse> GetOrderAsync(string commId);

        /// <summary>
        ///     Retrieves a list of all orders with updates after the provided timestamp.
        /// </summary>
        /// <param name="since"> Conmodo's system will keep track. If provided, retrieve order updated since provided date. </param>
        /// <returns> An object containing all updated orders. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code from Conmodo, or if we receive an invalid response object. </exception>
        public Task<UpdatedOrdersResponse> GetUpdatedOrdersAsync(DateTimeOffset? since);

        /// <summary>
        ///     Creates a new service-order.
        /// </summary>
        /// <param name="orderRequest"> The service-order that should be created. </param>
        /// <returns> The response object. </returns>
        /// <exception cref="HttpRequestException"> Thrown when we get a failure code from Conmodo, or if we receive an invalid response object. </exception>
        public Task<CreateOrderResponse> CreateServiceOrderAsync(CreateOrderRequest orderRequest);



    }
}
