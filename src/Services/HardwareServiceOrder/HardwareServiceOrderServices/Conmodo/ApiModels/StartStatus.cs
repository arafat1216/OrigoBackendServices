using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     This is used to sort the orders at Conmodo. 
    ///     A StartStatus is for instance Warranty, Insurance or similar. See "basedata API" for valid values.
    /// </summary>
    /// <see cref="Status"/>
    /// <see cref="BaseDataResponse"/>
    internal class StartStatus
    {
        [JsonPropertyName("startStatusID")]
        public int? StartStatusID { get; set; }

        [JsonPropertyName("startStatusName")]
        public string? StartStatusName { get; set; }

        [JsonPropertyName("subStartStatusID")]
        public int? SubStartStatusID { get; set; }

        [JsonPropertyName("subStartStatusName")]
        public string? SubStartStatusName { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        public StartStatus()
        {
        }

        /// <summary>
        ///     Instantiates a new start status, using the data we usually provide in new service-orders.
        /// </summary>
        /// <param name="startStatusID"> The <see cref="Status.Id">ID</see> for Conmodo's start-status. </param>
        /// <param name="startStatusName"> The <see cref="Status.Description">name</see> of Conmodo's start-status. </param>
        public StartStatus(int startStatusID, string startStatusName)
        {
            StartStatusID = startStatusID;
            StartStatusName = startStatusName;
        }
    }
}
