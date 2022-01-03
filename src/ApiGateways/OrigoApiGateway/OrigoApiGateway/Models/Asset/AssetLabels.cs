﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Request object
    /// </summary>
    public class AssetLabels
    {
        [Required]
        public IList<Guid> AssetGuids { get; set; }
        [Required]
        public IList<Guid> LabelGuids { get; set; }

        public Guid CallerId { get; set; }
    }
}
