namespace Common.Utilities
{
    public interface IFlatDictionaryProvider
    {
        Dictionary<string, string> Execute(object @object, string prefix = "");
    }
}
