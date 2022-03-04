using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class NewLabel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public LabelColor Color { get; set; }
    }
}
