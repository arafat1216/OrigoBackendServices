namespace SubscriptionManagementServices.Utilities
{
    interface IFlatDictionaryProvider
    {
        Dictionary<string, string> Execute(object @object, string prefix = "");
    }
}
