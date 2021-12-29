using Common.Enums;
using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class LabelDTO
    {
        public Guid Id { get; init; }

        public string Text { get; init; }

        public LabelColor Color { get; init; }
    }
}
