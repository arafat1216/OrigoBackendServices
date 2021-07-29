using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public Guid EventId { get; private set; }
        [Required]
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public IEvent FunctionalLogEvent { get; private set; }
        [Required]
        public EventStateEnum State { get; set; }
        [Required]
        public int TimesSent { get; set; }
        [Required]
        public DateTime CreationTime { get; private set; }
        [Required]
        public string Content { get; private set; }
        [Required]
        public string TransactionId { get; private set; }

        public FunctionalEventLogEntry DeserializeJsonContent(Type type)
        {
            FunctionalLogEvent = JsonSerializer.Deserialize(Content, type) as IEvent;
            return this;
        }
    }
}