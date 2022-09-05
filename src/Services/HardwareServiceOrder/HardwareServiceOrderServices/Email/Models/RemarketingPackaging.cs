namespace HardwareServiceOrderServices.Email.Models;

/// <summary>
/// Email Template Model for Asset Remarketing(Recycle and Wipe) including packaging service 
/// </summary>
public class RemarketingPackaging
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
    /// Address of the recipient
    /// </summary>
    public string Address { get; set; }
    
    /// <summary>
    /// Email address of the recipient
    /// </summary>
    public string Recipient { get; set; }

    public string Subject { get; set; }
    public const string TemplateKeyName = "RemarketingPackaging_Body";
    public const string SubjectKeyName = "RemarketingPackaging_Subject";
}