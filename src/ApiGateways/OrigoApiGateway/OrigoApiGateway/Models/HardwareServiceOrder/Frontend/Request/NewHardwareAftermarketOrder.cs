using System.Text.Json.Serialization;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    /// <summary>
    ///     A override of <see cref="NewHardwareServiceOrder"/> where the <see cref="UserDescription"/> property
    ///     has been overwritten so it is hidden from the API. The overwritten value is also assigned a fixed value.
    /// </summary>
    public class NewHardwareAftermarketOrder : NewHardwareServiceOrder
    {
        public NewHardwareAftermarketOrder()
        {
            base.UserDescription = "Device return";
        }


        /// <inheritdoc cref="NewHardwareServiceOrder.UserDescription"/>
        [JsonIgnore]
        public new string UserDescription
        {
            set { base.UserDescription = value; }
            get { return base.UserDescription; }
        }
    }
}
