using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Models.Database.Interfaces;

namespace ProductCatalog.Infrastructure.Models.Database
{
    public abstract class Entity : IEntityFrameworkEntity
    {
        /*
         * EF DB Columns
         */

        /// <summary>
        ///     For write-operations this is the ID of the user that's currently performing the update or creation. 
        ///     For read-operations this is ID of the user that updated the returned record.
        /// </summary>
        [Comment("The ID of the user that performed the last update.")]
        public Guid UpdatedBy { get; set; }

        /*
         * Constructors
         */

        [Obsolete("This is a reserved constructor that should only be utilized by the automated Entity Framework injections! Make sure you are using the correct \"base()\" constructor.", false)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "Entity Framework requires a public default constructor")]
        public Entity()
        {
        }


        protected Entity(Guid updatedBy)
        {
            UpdatedBy = updatedBy;
        }
    }
}
