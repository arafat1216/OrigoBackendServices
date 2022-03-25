using System;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents;

public class AssignContractHolderToAssetLifeCycleDomainEvent : BaseEvent
{
    public AssignContractHolderToAssetLifeCycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, User? previousContractHolderUser) : base(
        assetLifecycle.ExternalId)
    {
        AssetLifecycle = assetLifecycle;
        CallerId = callerId;
        PreviousContractHolder = previousContractHolderUser;
    }

    public AssetLifecycle AssetLifecycle { get; protected set; }
    public Guid CallerId { get; protected set; }
    public User? PreviousContractHolder { get; protected set; }

    public override string EventMessage()
    {
        return PreviousContractHolder == null
            ? $"Asset lifecycle assigned to user {AssetLifecycle?.ContractHolderUser?.Name}."
            : $"Asset lifecycle assigned to asset {AssetLifecycle?.ContractHolderUser?.Name} - {AssetLifecycle?.ContractHolderUser?.Name} ({AssetLifecycle?.ContractHolderUser?.ExternalId}) from user {PreviousContractHolder.Name} ({PreviousContractHolder.ExternalId}).";
    }
}