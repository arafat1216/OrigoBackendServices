using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Asset.API.ViewModels
{
    public class AssetLabels
    {
        [Required]
        public IList<Guid> AssetGuids { get; set; }
        [Required]
        public IList<Guid> LabelGuids { get; set; }

        public Guid CallerId { get; set; }
    }
}
