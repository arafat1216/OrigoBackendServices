namespace SubscriptionManagementServices.Utilities
{
   public interface IFlatDictionaryProvider
    {
        Dictionary<string, string> Execute(object @object, string prefix = "");
    }
}
