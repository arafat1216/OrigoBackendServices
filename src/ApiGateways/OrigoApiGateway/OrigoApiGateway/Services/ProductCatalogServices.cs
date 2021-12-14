using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models.ProductCatalog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class ProductCatalogServices : IProductCatalogServices
    {
        private readonly ILogger<ProductCatalogServices> _logger;
        private readonly ProductCatalogConfiguration _options;

        private HttpClient HttpClient { get; }
        private string ProductsApiPath { get; }
        private string ProductTypesApiPath { get; }
        private string OrdersApiPath { get; }
        private string FeaturesApiPath { get; }


        public ProductCatalogServices(ILogger<ProductCatalogServices> logger, HttpClient httpClient, IOptions<ProductCatalogConfiguration> options)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;

            ProductsApiPath = $"{_options.ApiPath}products";
            ProductTypesApiPath = $"{_options.ApiPath}producttypes";
            OrdersApiPath = $"{_options.ApiPath}orders";
            FeaturesApiPath = $"{_options.ApiPath}features";
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

        public async Task<IEnumerable<ProductGet>> GetOrdersByPartnerAndOrganizationAsync(Guid organizationId)
        {
            return await GetAsync<IEnumerable<ProductGet>>($"{OrdersApiPath}/organization/{organizationId}", nameof(GetOrdersByPartnerAndOrganizationAsync));
        }


        public async Task ReplaceOrderedProductsAsync(Guid partnerId, Guid organizationId, ProductOrdersDTO updateProductOrders)
        {
            await PutAsync($"{OrdersApiPath}/partner/{partnerId}/organization/{organizationId}", updateProductOrders, nameof(ReplaceOrderedProductsAsync));
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"> The object that is de-serialized and used for the PUT request. </typeparam>
        /// <param name="path"></param>
        /// <param name="inputEntity"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        /// <exception cref="MicroserviceErrorResponseException"></exception>
        /// <exception cref="Exception"> An unexpected exception occurred. </exception>
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
                _logger.LogError(exception, $"{methodName} encountered an 'HttpRequestException'. Unique location ID: 55C012CD-2E59-458B-AEDC-E0EE94E4B4FB");
                throw new MicroserviceErrorResponseException(string.Empty, HttpStatusCode.GatewayTimeout);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"{methodName} encountered an unexpected exception. Unique location ID: EAF80475-9430- 41C7-B2B0-C5AF74ACC443");
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutput"> The object that is serialized, and retrieved from the GET request. </typeparam>
        /// <param name="path"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        /// <exception cref="MicroserviceErrorResponseException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"> An unexpected exception occurred. </exception>
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
                _logger.LogError(exception, $"{methodName} encountered an 'HttpRequestException'. Unique location ID: 44498A62-4A97-4B36-8AE1-D9511DB06D52");
                throw new MicroserviceErrorResponseException(string.Empty, HttpStatusCode.GatewayTimeout);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"{methodName} encountered an unexpected exception. Unique location ID: 72F74E81-99D2-4246-BBAC-0E3B95CB7C7E");
                throw;
            }
        }
    }
}
