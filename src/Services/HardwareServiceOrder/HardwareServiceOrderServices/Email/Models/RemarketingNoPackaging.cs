namespace HardwareServiceOrderServices.Email.Models;

/// <summary>
/// Email Template Model for Asset Remarketing(Recycle and Wipe) with No packaging service 
/// </summary>
public class RemarketingNoPackaging
{
    /// <summary>
    /// First name of the email recipient
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Name of the asset
    /// </summary>
    public string AssetName { get; set; }

    /// <summary>
    /// Unique identifier of the asset
    /// </summary>
    public Guid AssetId { get; set; }

    /// <summary>
    /// Date of the Order
    /// </summary>
    public DateTime OrderDate { get; set; }
    
    /// <summary>
    /// Email address of the recipient
    /// </summary>
    public string Recipient { get; set; }
    
    public string Subject { get; set; }
    public const string TemplateKeyName = "RemarketingNoPackaging_Body";
    public const string SubjectKeyName = "RemarketingNoPackaging_Subject";
}