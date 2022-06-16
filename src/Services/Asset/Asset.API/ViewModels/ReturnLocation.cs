using System;

namespace Asset.API.ViewModels
{
    public class ReturnLocation
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string ReturnDescription { get; init; }
        public Guid LocationId { get; init; }
    }
}
