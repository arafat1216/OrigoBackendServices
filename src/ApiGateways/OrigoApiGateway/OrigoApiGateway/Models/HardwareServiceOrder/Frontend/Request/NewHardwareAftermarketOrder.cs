using System.Text.Json.Serialization;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    /// <summary>
    ///     A override of <see cref="NewHardwareServiceOrder"/>, where the <see cref="UserDescription"/> property
    ///     is overwritten so it's hidden from the API, and has been set to a default value.
    /// </summary>
    public class NewHardwareAftermarketOrder : NewHardwareServiceOrder
    {
        public NewHardwareAftermarketOrder()
        {
            base.UserDescription = string.Empty;
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
