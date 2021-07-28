using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using MediatR;

namespace Common.Logging
{

    public class FunctionalEventLogEntry
    {
        private FunctionalEventLogEntry() { }
        public FunctionalEventLogEntry(IEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonSerializer.Serialize(@event as object);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }

        public int Id { get; set; }
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public IEvent FunctionalLogEvent { get; private set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
        public string TransactionId { get; private set; }

        public FunctionalEventLogEntry DeserializeJsonContent(Type type)
        {
            FunctionalLogEvent = JsonSerializer.Deserialize(Content, type) as IEvent;
            return this;
        }
    }
}