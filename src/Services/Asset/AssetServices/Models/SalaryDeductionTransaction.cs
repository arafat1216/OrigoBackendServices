using Common.Enums;
using Common.Seedwork;

namespace AssetServices.Models;

/// <summary>
/// Handles a transaction linked to salary deduction for one asset lifecycle.
/// This will be a list of historic and future transactions related to a salary deduction
/// for an employee holding this asset lifecycle.
/// </summary>
public class SalaryDeductionTransaction : Entity
{
    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Amount { get; set; }

    public CurrencyCode CurrencyCode { get; set; }

    /// <summary>
    /// If the employee no longer works for the company or the asset lifecycle ends prematurely
    /// this should be set.
    /// </summary>
    public bool Cancelled { get; set; }
}