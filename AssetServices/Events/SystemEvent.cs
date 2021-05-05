namespace AssetServices.Events
{
    public class SystemEvent : BaseEvent
    {
        public SystemEvent(){
            EventType = "System";
        }
    }
}