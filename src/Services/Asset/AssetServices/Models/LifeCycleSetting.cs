using System;
using AssetServices.DomainEvents;
using Common.Seedwork;

namespace AssetServices.Models;

public class LifeCycleSetting : Entity
{
    public LifeCycleSetting(int assetCategoryId, bool buyoutAllowed, decimal minBuyoutPrice, int runtime, Guid callerId)
    {
        BuyoutAllowed = buyoutAllowed;
        MinBuyoutPrice = minBuyoutPrice;
        AssetCategoryId = assetCategoryId;
        CreatedBy = callerId;
        Runtime = runtime;
    }

    public LifeCycleSetting()
    {
    }

    /// <summary>
    ///     The external uniquely identifying id across systems.
    /// </summary>
    public Guid ExternalId { get; private set; } = Guid.NewGuid();
    /// <summary>
    ///     Is buyout feature on or off for this customer.
    /// </summary>
    public bool BuyoutAllowed { get; protected set; }

    /// <summary>
    /// the min buyout price for this category and for this customer.
    /// </summary>
    public decimal MinBuyoutPrice { get; protected set; } = decimal.Zero;

    /// <summary>
    /// The category id that the setting is for.
    /// </summary>
    public int AssetCategoryId { get; init; }

    /// <summary>
    /// the runtime month for this category and for this customer.
    /// </summary>
    public int Runtime { get; protected set; }

    /// <summary>
    /// Return the name of the category based on the Category Id.
    /// </summary>
    public string AssetCategoryName
    {
        get
        {
            return AssetCategoryId switch
            {
                1 => "Mobile phone",
                2 => "Tablet",
                _ => "Unknown"
            };
        }
    }

    /// <summary>
    ///     Update Buyout Allowed property for this setting.
    /// </summary>
    /// <param name="buyoutAllowed">The data for updating buyoutAllowed</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void SetBuyoutAllowed(bool buyoutAllowed, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousStatus = BuyoutAllowed;
        BuyoutAllowed = buyoutAllowed;
        AddDomainEvent(new SetBuyoutAllowedDomainEvent(this, callerId, previousStatus));
    }

    /// <summary>
    ///     Set Min Buyout Price for this setting and Category.
    /// </summary>
    /// <param name="buyoutPrice">The amount that will be set min buyout price</param>
    /// <param name="categoryId">The specific Asset Category Id for this setting</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void SetMinBuyoutPrice(decimal buyoutPrice, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        MinBuyoutPrice = buyoutPrice;
        AddDomainEvent(new SetBuyoutPriceDomainEvent(this, callerId));
    }

    /// <summary>
    ///     Set Asset Runtime for this setting and Category.
    /// </summary>
    /// <param name="runtime">The amount that will be set min buyout price</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void SetLifeCycleRuntime(int runtime, Guid callerId)
    {
        var previousRuntime = Runtime;
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        Runtime = runtime;
        AddDomainEvent(new SetRuntimeDomainEvent(this, callerId, previousRuntime));
    }
}