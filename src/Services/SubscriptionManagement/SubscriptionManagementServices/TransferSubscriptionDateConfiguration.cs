namespace SubscriptionManagementServices
{
    public class TransferSubscriptionDateConfiguration
    {
        public int MaxDaysForAll { get; set; }
        public int MinDaysForCurrentOperator { get; set; }
        public int MinDaysForNewOperator { get; set; }
        public int MinDaysForNewOperatorWithSIM { get; set; }
    }
}
