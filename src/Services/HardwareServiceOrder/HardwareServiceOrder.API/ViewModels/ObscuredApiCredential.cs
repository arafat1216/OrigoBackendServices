namespace HardwareServiceOrder.API.ViewModels
{
    [SwaggerSchema(ReadOnly = true)]
    public class ObscuredApiCredential
    {
        /// <summary>
        ///     The ID of the service-type this API credential is valid for.
        /// </summary>
        [Required]
        public int ServiceTypeId { get; init; }

        [Required]
        public bool ApiUsernameFilled;

        [Required]
        public bool ApiPasswordFilled;
    }
}
