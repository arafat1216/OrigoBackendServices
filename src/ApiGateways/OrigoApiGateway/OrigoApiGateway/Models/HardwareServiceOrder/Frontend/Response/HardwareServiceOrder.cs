#nullable enable

using Common.Seedwork;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    public class HardwareServiceOrder
    {
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public Guid Id { 
            get; 
            set; 
        }

        [Obsolete("A temp. backend-only quickfix. Should not be used by frontend as it will soon be removed!")]
        public Guid ExternalId
        {
            set
            {
                if (Id == Guid.Empty)
                    Id = value;
            }
        }

        /// <summary>
        /// Customer identifier
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Asset life-cycle identifier
        /// </summary>
        [Required]
        public Guid AssetLifecycleId { get; set; }

        /// <summary>
        /// Asset life-cycle category identifier
        /// </summary>
        [Required]
        public int AssetLifecycleCategoryId { get; set; }

        /// <summary>
        /// Asset details
        /// </summary>
        [Required]
        public AssetInfo AssetInfo { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public AssetInfo? ReturnedAssetInfo { get; set; }

        /// <summary>
        /// Fault description
        /// </summary>
        [Required]
        public string UserDescription { get; set; }

        /// <summary>
        /// Details of the order ownder
        /// </summary>
        [Required]
        public ContactDetails Owner { get; set; }

        /// <summary>
        /// Delivery address of the order
        /// </summary>
        [Required]
        public DeliveryAddress? DeliveryAddress { get; set; }

        /// <summary>
        ///     The identifier for the service-type that has been registered on this service-order.
        /// </summary>
        [Required]
        public int ServiceTypeId { get; set; }

        /// <summary>
        ///     The identifier for the service-order's current status.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int StatusId { get; set; }

        /// <summary>
        ///     The identifier for service-provider that handles this service-order.
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        /// Link to the external service management portal
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        public string? ExternalServiceManagementLink { get; set; }

        /// <summary>
        /// All single events/updates for a service-order that has been received and adapted from the service-provider.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public IEnumerable<ServiceEvent> ServiceEvents { get; set; }

        /// <inheritdoc cref="Auditable.DateCreated"/>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset DateCreated { get; init; }

        /// <inheritdoc cref="Auditable.CreatedBy"/>
        [SwaggerSchema(ReadOnly = true)]
        public Guid CreatedBy { get; init; }

        /// <inheritdoc cref="Auditable.DateUpdated"/>
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset? DateUpdated { get; init; }

        /// <inheritdoc cref="Auditable.UpdatedBy"/>
        [SwaggerSchema(ReadOnly = true)]
        public Guid? UpdatedBy { get; init; }



        // TODO: Below this point is temp. remapped or hardcoded entries and is purely for not breaking the frontend while the refactoring is ongoing.


        [Obsolete("This is removed and replaced with the 'StatusId' identifier.")]
        [SwaggerSchema(ReadOnly = true)]
        public string? Status { get; }

        [Obsolete("This is removed and replaced with the 'ServiceProviderId' identifier. It will temporarily allow for mapping between them, but is limited to 'ConmodoNo'.")]
        public string? ServiceProvider
        {
            get
            {
                if (ServiceProviderId == 1) return "ConmodoNo";
                else return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Equals("ConmodoNo"))
                {
                    ServiceProviderId = 1;
                }
            }
        }


        [Obsolete("This is removed and replaced with the 'TypeId' identifier. It will temporarily allow for mapping between them, but is limited to 'SUR'.")]
        public string? Type
        {
            get
            {
                if (ServiceTypeId == 3) return "SUR";
                else return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Equals("SUR"))
                {
                    ServiceTypeId = 3;
                }
            }
        }

        //[Obsolete("This have been removed. Use 'Owner.UserId' instead.")]
        //[SwaggerSchema(ReadOnly = true)]
        //public Guid? Owner { get; } // get from somewhere else

        [Obsolete("Renamed to 'ServiceEvents'. This temporary alias will be removed in an upcoming commit.")]
        [SwaggerSchema(ReadOnly = true)]
        public IEnumerable<ServiceEvent> Events { get { return ServiceEvents; } }

        [Obsolete("Renamed to 'UserDescription'. This temporary alias will be removed in an upcoming commit.")]
        [SwaggerSchema(ReadOnly = true)]
        public string ErrorDescription { get { return UserDescription; } }

        [Obsolete("Renamed to 'DateCreated'. This temporary alias will be removed in an upcoming commit.")]
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset Created { get { return DateCreated; } }

        [Obsolete("Renamed to 'DateUpdated'. This temporary alias will be removed in an upcoming commit.")]
        [SwaggerSchema(ReadOnly = true)]
        public DateTimeOffset? Updated { get { return DateUpdated; } }
    }
}