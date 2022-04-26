namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a object that contains a list of translations.
    /// </summary>
    /// <typeparam name="TEntity"> A <see cref="ITranslationResult"/> item, or a class that implements this interface. </typeparam>
    /// <seealso cref="ITranslationResult"/>
    public interface ITranslatable<TEntity> where TEntity : ITranslationResult
    {
        /// <summary>
        ///     A collection that contains the translations for this entity-type.
        /// </summary>
        ICollection<TEntity> Translations { get; set; }
    }
}
