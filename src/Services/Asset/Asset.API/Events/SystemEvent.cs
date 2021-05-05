namespace Asset.API.Events
{
    public class SystemEvent : BaseEvent
    {
        public SystemEvent(){
            EventType = "System";
        }
    }
}