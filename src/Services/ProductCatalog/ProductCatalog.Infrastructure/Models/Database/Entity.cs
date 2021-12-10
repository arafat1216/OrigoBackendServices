using ProductCatalog.Infrastructure.Models.Database.Interfaces;

namespace ProductCatalog.Infrastructure.Models.Database
{
    public abstract class Entity : IEntityFrameworkEntity
    {
        // EF DB Columns
        public Guid UpdatedBy { get; set; }


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
