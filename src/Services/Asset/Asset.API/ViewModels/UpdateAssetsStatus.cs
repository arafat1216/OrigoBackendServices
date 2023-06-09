﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    /// <summary>
    /// A list of Assets ID (Guid), and an integer representing the status these assets will be set to.
    /// </summary>
    public class UpdateAssetsStatus
    {
        [Required]
        public IList<Guid> AssetGuidList { get; set; }

        /// <summary>
        /// id of user making the endpoint call. Can be ignored by frontend.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
