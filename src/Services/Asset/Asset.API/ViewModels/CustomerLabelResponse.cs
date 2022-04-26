using System;

namespace Asset.API.ViewModels
{
    public class CustomerLabelResponse
    {
        public CustomerLabelResponse()
        {
        }

        public Guid ExternalId { get; }
        public Guid CustomerId { get; }
        public Label Label { get; }
    }
}
