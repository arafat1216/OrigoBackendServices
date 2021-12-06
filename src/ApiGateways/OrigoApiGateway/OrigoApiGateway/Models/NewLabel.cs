using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class NewLabel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public LabelColor Color { get; set; }
    }
}
