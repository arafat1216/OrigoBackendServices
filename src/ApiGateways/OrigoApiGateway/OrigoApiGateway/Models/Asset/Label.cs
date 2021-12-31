using Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class Label
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        public string Text { get; init; }

        [Required]
        public LabelColor Color { get; init; }

        public string ColorName
        {
            get => Enum.GetName(Color);
        }
    }
}
