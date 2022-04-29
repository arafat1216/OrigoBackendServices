using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class AddDocumentRequest
    {
        /// <summary>
        ///     A base64-encoded string
        /// </summary>
        [Required]
        [JsonPropertyName("fileData")]
        public string FileData { get; set; }

        /// <summary>
        ///     Order number for the file to be added to
        /// </summary>
        [Required]
        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> kvittering.pdf </example>
        [Required]
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <example> pdf </example>
        [Required]
        [JsonPropertyName("fileType")]
        public string FileType { get; set; }

        /// <summary>
        ///     Your reference shown on the order label
        /// </summary>
        [Required]
        [JsonPropertyName("commId")]
        public string CommId { get; set; }
    }
}
