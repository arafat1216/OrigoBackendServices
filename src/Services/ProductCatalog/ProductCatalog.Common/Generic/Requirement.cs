﻿using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Common.Generic
{
    /// <summary>
    ///     Defines other entries that must - or can't - be combined with this one.
    ///     
    /// 
    ///     The requirements for all three categories must be fulfilled.
    /// </summary>
    public class Requirement
    {
        /// <summary>
        ///     The current item excludes all these items. This means that none of these items can be added alongside the current item.
        /// </summary>
        [Required]
        public IEnumerable<int> Excludes { get; set; }

        /// <summary>
        ///     The current item requires all these items to be present. This means that all corresponding items must be added alongside the current item.
        /// </summary>
        [Required]
        public IEnumerable<int> RequiresAll { get; set; }

        /// <summary>
        ///     The current item also requires one of these items to be present. This means that at least one of the corresponding items must also be added 
        ///     alongside the current item, but it doesn't matter which one.
        /// </summary>
        [Required]
        public IEnumerable<int> RequiresOne { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Requirement"/> class.
        /// </summary>
        /// <param name="excludes"> A list of excluded items. </param>
        /// <param name="requiresAll"> A list of items, where all entries are required. </param>
        /// <param name="requiresOne"> A list of items, where at least one entry is required. </param>
        public Requirement(IEnumerable<int> excludes, IEnumerable<int> requiresAll, IEnumerable<int> requiresOne)
        {
            Excludes = excludes;
            RequiresAll = requiresAll;
            RequiresOne = requiresOne;
        }
    }
}
