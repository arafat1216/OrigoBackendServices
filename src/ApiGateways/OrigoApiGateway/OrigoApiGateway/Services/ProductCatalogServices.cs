using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models.ProductCatalog;
using System.Net;

namespace OrigoApiGateway.Services
{
    public class ProductCatalogServices : IProductCatalogServices
    {
        private readonly ILogger<ProductCatalogServices> _logger;
        private readonly ProductCatalogConfiguration _options;

        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient HttpClient => _httpClientFactory.CreateClient("productcatalogservices");

        private string ProductsApiPath { get; }
        private string ProductTypesApiPath { get; }
        private string OrdersApiPath { get; }
        private string FeaturesApiPath { get; }


        public ProductCatalogServices(ILogger<ProductCatalogServices> logger, IHttpClientFactory httpClientFactory, IOptions<ProductCatalogConfiguration> options)
        {
            _logger = logger;
            _options = options.Value;

            ProductsApiPath = $"{_options.ApiPath}products";
            ProductTypesApiPath = $"{_options.ApiPath}producttypes";
            OrdersApiPath = $"{_options.ApiPath}orders";
            FeaturesApiPath = $"{_options.ApiPath}features";
            _httpClientFactory = httpClientFactory;
        }

        #region Features

        public async Task<IEnumerable<string>> GetProductPermissionsForOrganizationAsync(Guid organizationId)
        {
            return await GetAsync<IEnumerable<string>>($"{FeaturesApiPath}/organization/{organizationId}/permissions", nameof(GetProductPermissionsForOrganizationAsync));
        }

        #endregion


        #region Products

        public async Task<ProductGet> GetProductByIdAsync(int productId)
        {
            return await GetAsync<ProductGet>($"{ProductsApiPath}/{productId}", nameof(GetProductByIdAsync));
        }

        public async Task<IEnumerable<ProductGet>> GetAllProductsByPartnerAsync(Guid partnerId)
        {
            return await GetAsync<IEnumerable<ProductGet>>($"{ProductsApiPath}/partner/{partnerId}", nameof(GetAllProductsByPartnerAsync));
        }

        #endregion


        #region Product Types

        public async Task<IEnumerable<ProductTypeGet>> GetAllProductTypesAsync()
        {
            return await GetAsync<IEnumerable<ProductTypeGet>>(ProductTypesApiPath, nameof(GetAllProductTypesAsync));
        }

        #endregion


        #region Orders

        public async Task<IEnumerable<ProductGet>> GetOrderedProductsByPartnerAndOrganizationAsync(Guid partnerId, Guid organizationId)
        {
            return await GetAsync<IEnumerable<ProductGet>>($"{OrdersApiPath}/partner/{partnerId}/organization/{organizationId}", nameof(GetOrderedProductsByPartnerAndOrganizationAsync));
        }


        public async Task ReplaceOrderedProductsAsync(Guid partnerId, Guid organizationId, ProductOrdersDTO productOrders)
        {
            await PutAsync($"{OrdersApiPath}/partner/{partnerId}/organization/{organizationId}", productOrders, nameof(ReplaceOrderedProductsAsync));
        }

        #endregion


        /// <summary>
        ///     Performs a PUT request to the underlaying micro-service. If we get an error-code back, it maps it to a
        ///     corresponding <see cref="MicroserviceErrorResponseException"/> and throws it so the controller or an interceptor can handle it.
        /// </summary>
        /// <typeparam name="TInput"> The type of object that is serialized and used for the PUT request. </typeparam>
        /// <param name="path"> The URI to the endpoint. </param>
        /// <param name="inputEntity"> The entity that is used for the request. </param>
        /// <param name="methodName"> The name of the class that is calling this method. This is used for exception logging. <para>
        /// 
        ///     Example: '<c>nameof(myMethodName)</c>'. </para></param>
        /// <returns> The <see langword="async"/> <see cref="Task"/>. </returns>
        /// <exception cref="MicroserviceErrorResponseException"> Thrown when we had problems with the HTTP request, 
        ///     or if the micro-service returned an error code. </exception>
        /// <exception cref="Exception"> Thrown when an unexpected exception occurred. </exception>
        public async Task PutAsync<TInput>(string path, TInput inputEntity, string methodName) where TInput : notnull
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync(path, inputEntity);

                if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.MultiStatus)
                {
                    throw new MicroserviceErrorResponseException(response.ReasonPhrase, response.StatusCode);
                }
            }
            catch (MicroserviceErrorResponseException)
            {
                // It's expected since we just threw it. Duhh.. Re-throw it as intended, so we can intercept it in the controller.
                throw;
            }
            // Thrown by HttpClient when there are connection issues. Let's re-package it as an API Exception so we can throw it to the controller for proper handling.
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "{0} encountered an 'HttpRequestException'. Unique location ID: 55C012CD-2E59-458B-AEDC-E0EE94E4B4FB", methodName);
                throw new MicroserviceErrorResponseException(string.Empty, HttpStatusCode.GatewayTimeout);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{0} encountered an unexpected exception. Unique location ID: EAF80475-9430- 41C7-B2B0-C5AF74ACC443", methodName);
                throw;
            }
        }


        /// <summary>
        ///     Performs a GET request to the underlaying micro-service. If we get an error-code back, it maps it to a
        ///     corresponding <see cref="MicroserviceErrorResponseException"/> and throws it so the controller or an interceptor can handle it.
        /// </summary>
        /// <typeparam name="TOutput"> The type of object that is deserialized, and retrieved from the GET request. </typeparam>
        /// <param name="path"> The URI to the endpoint. </param>
        /// <param name="methodName"> The name of the class that is calling this method. This is used for exception logging. <para>
        /// 
        ///     Example: '<c>nameof(myMethodName)</c>'. </para></param>
        /// <returns> The deserialized object as <typeparamref name="TOutput"/>. </returns>
        /// <exception cref="MicroserviceErrorResponseException"> Thrown when we had problems with the HTTP request, 
        ///     or if the micro-service returned an error code. </exception>
        /// <exception cref="Exception"> Thrown when an unexpected exception occurred. </exception>
        public async Task<TOutput> GetAsync<TOutput>(string path, string methodName) where TOutput : notnull
        {
            try
            {
                var response = await HttpClient.GetAsync(path);

                if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.MultiStatus)
                {
                    throw new MicroserviceErrorResponseException(response.ReasonPhrase, response.StatusCode);
                }
                else
                {
#if DEBUG
                    var deserialized = await response.Content.ReadAsStringAsync();
#endif
                    TOutput result = await response.Content.ReadFromJsonAsync<TOutput>();
                    return result;
                }
            }
            catch (MicroserviceErrorResponseException)
            {
                // It's expected since we just threw it. Duhh.. Re-throw it as intended, so we can intercept it in the controller.
                throw;
            }
            // Thrown by HttpClient when there are connection issues. Let's re-package it as an API Exception so we can throw it to the controller for proper handling.
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "{0} encountered an 'HttpRequestException'. Unique location ID: 44498A62-4A97-4B36-8AE1-D9511DB06D52", methodName);
                throw new MicroserviceErrorResponseException(string.Empty, HttpStatusCode.GatewayTimeout);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{0} encountered an unexpected exception. Unique location ID: 72F74E81-99D2-4246-BBAC-0E3B95CB7C7E", methodName);
                throw;
            }
        }
    }
}
