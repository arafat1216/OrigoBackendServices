namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    /// <summary>
    ///     A Conmodo specific exception that is thrown when the event-mapping requires the solution to change to the historical mapping implementation.
    /// </summary>
    public class ConmodoEventMappingException : Exception
    {
        public ConmodoEventMappingException() { }
        public ConmodoEventMappingException(string message) : base(message) { }
        public ConmodoEventMappingException(string message, Exception inner) : base(message, inner) { }
    }
}
