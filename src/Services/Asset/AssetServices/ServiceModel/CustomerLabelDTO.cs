using System;

namespace AssetServices.ServiceModel;

public record CustomerLabelDTO
{
    public CustomerLabelDTO(Guid externalId, Guid customerId, LabelDTO label)
    {
        ExternalId = externalId;
        CustomerId = customerId;
        Label = label;
    }

    public Guid ExternalId { get; }
    public Guid CustomerId { get; }
    public LabelDTO Label { get; }

}