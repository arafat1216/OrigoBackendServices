using System;
using System.Text.Json.Serialization;
using AssetServices.Models;
using Common.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace AssetServices.DomainEvents.AssetLifecycleEvents;

public class AssignContractHolderToAssetLifeCycleDomainEvent : BaseEvent
{
    public AssignContractHolderToAssetLifeCycleDomainEvent(AssetLifecycle assetLifecycle, Guid callerId, User? previousContractHolder) : base(
        assetLifecycle.ExternalId)
    {
        AssetLifecycle = assetLifecycle;
        CallerId = callerId;
        PreviousContractHolder = previousContractHolder;
    }

    public AssetLifecycle AssetLifecycle { get; protected set; }
    public Guid CallerId { get; protected set; }
    public User? PreviousContractHolder { get; protected set; }

    public override string EventMessage()
    {
        return AssetLifecycle?.ContractHolderUser == null
            ? $"Asset lifecycle assigned to user."
            : $"Asset lifecycle assigned to asset {AssetLifecycle?.ContractHolderUser?.Name} - {AssetLifecycle?.ContractHolderUser?.Name} ({AssetLifecycle?.ContractHolderUser?.ExternalId}) from user {PreviousContractHolder.Name} ({PreviousContractHolder.ExternalId}).";
    }
}