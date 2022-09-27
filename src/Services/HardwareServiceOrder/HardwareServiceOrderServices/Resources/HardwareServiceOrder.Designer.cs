﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HardwareServiceOrderServices.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class HardwareServiceOrder {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal HardwareServiceOrder() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", typeof(HardwareServiceOrder).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///
        ///In connection to a recent repair order your asset was discarded. Please contact your manager to get a new asset..
        /// </summary>
        internal static string AssetDiscardedEmail {
            get {
                return ResourceManager.GetString("AssetDiscardedEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace Discarded Asset.
        /// </summary>
        internal static string AssetDiscardedEmail_Subject {
            get {
                return ResourceManager.GetString("AssetDiscardedEmail_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///
        ///You registered a [repair order]({{OrderLink}}) [{{OrderDate}}], but we cannot see having received the asset. Please follow the instructions below so we can help you fix it!
        ///
        ///Next steps:
        ///
        ///1. Backup your device
        ///2. Factory reset your device
        ///3. Remove the SIM card
        ///
        ///⚠️ if step 1 to 3 is not done properly the repair cannot be completed
        ///
        ///4. Send the device to the repair provider using the package slip
        ///5. The repair provider evaluates your device
        ///6. You get repair options to choo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AssetRepairEmail {
            get {
                return ResourceManager.GetString("AssetRepairEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Repair Reminder.
        /// </summary>
        internal static string AssetRepairEmail_Subject {
            get {
                return ResourceManager.GetString("AssetRepairEmail_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///
        ///I connection to a recent repair order we would like to remind you to return any loan device..
        /// </summary>
        internal static string LoanDeviceEmail {
            get {
                return ResourceManager.GetString("LoanDeviceEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Return Loan Device.
        /// </summary>
        internal static string LoanDeviceEmail_Subject {
            get {
                return ResourceManager.GetString("LoanDeviceEmail_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string OngoingUserActionNeeded_Body {
            get {
                return ResourceManager.GetString("OngoingUserActionNeeded_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string OngoingUserActionNeeded_Subject {
            get {
                return ResourceManager.GetString("OngoingUserActionNeeded_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///
        ///Your repair order is canceled. If your asset still needs repair please create a new repair order.
        ///
        ///
        ///### **Order details:**
        ///
        ///Assets: {{AssetName}} ({{AssetId}})  
        ///Order date: {{OrderDate}}  
        ///Repair type: {{RepairType}}  
        ///Fault category: {{FaultCategory}}
        ///
        ///[View order in Origo]({{OrderLink}}).
        /// </summary>
        internal static string OrderCancellationEmail {
            get {
                return ResourceManager.GetString("OrderCancellationEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Canceled Repair Order.
        /// </summary>
        internal static string OrderCancellationEmail_Subject {
            get {
                return ResourceManager.GetString("OrderCancellationEmail_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///
        ///### **Order details:**
        ///
        ///Assets: {{AssetName}} ({{AssetId}})  
        ///Order date: {{OrderDate}}  
        ///Repair type: {{RepairType}}  
        ///Fault category: {{FaultCategory}}
        ///
        ///[View order in Origo]({{OrderLink}})
        ///
        ///### **Next steps:** 
        ///1. Backup your device
        ///2. Factory reset your device
        ///3. Remove SIM card
        ///
        ///⚠️ if step 1 to 3 is not done properly the repair cannot be completed
        ///
        ///4. Send the device to the repair provider using the package slip
        ///5. The repair provider evaluates your device
        ///6. Y [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string OrderConfirmationEmail {
            get {
                return ResourceManager.GetString("OrderConfirmationEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string RegisteredUserActionNeeded_Body {
            get {
                return ResourceManager.GetString("RegisteredUserActionNeeded_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string RegisteredUserActionNeeded_Subject {
            get {
                return ResourceManager.GetString("RegisteredUserActionNeeded_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{ FirstName }}!
        ///
        ///
        ///### Return details:
        ///Asset: {{ AssetName }} {{ AssetId }}
        ///Date: {{ OrderDate }}
        ///
        ///### Next steps:
        ///1. Transfer all data from the old asset to the new asset
        ///2. Factory reset/delete all data from the old asset
        ///3. (if iPhone) Deactivate Find my iPhone
        ///4. Remove the SIM card/memory card
        ///5. Package and send asset
        ///
        ///If step 1 to 4 is not done properly the recycle cannot be completed
        ///
        ///### How to package and send the asset:
        ///Please package your device securely so tha [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RemarketingNoPackaging_Body {
            get {
                return ResourceManager.GetString("RemarketingNoPackaging_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Repair Order.
        /// </summary>
        internal static string RemarketingNoPackaging_Subject {
            get {
                return ResourceManager.GetString("RemarketingNoPackaging_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{ FirstName }}!
        ///&amp;nbsp;
        ///&amp;nbsp;
        ///### Return details:
        ///Asset: {{ AssetName }} {{ AssetId }}
        ///Date: {{ OrderDate }}
        ///
        ///### Would you like to receive packaging?
        ///Yes
        ///Address: {{ Address }}
        ///
        ///### Next steps:
        ///1. Receive return packaging and shipping label
        ///2. Transfer all data from the old asset to the new asset
        ///3. Factory reset/delete all data from the old asset
        ///4. (if iPhone) Deactivate Find my iPhone
        ///5. Remove the SIM card/memory card
        ///6. Ship the asset using the return package
        ///
        ///If step 2 to 5  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RemarketingPackaging_Body {
            get {
                return ResourceManager.GetString("RemarketingPackaging_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Repair Order (Included Packaging Service).
        /// </summary>
        internal static string RemarketingPackaging_Subject {
            get {
                return ResourceManager.GetString("RemarketingPackaging_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi There,
        ///
        ///The following order has received unknown status from the service provider. Please have a look.
        ///
        ///[View Order](OrderLink)
        ///
        ///{{Order}}.
        /// </summary>
        internal static string Unknown_Body {
            get {
                return ResourceManager.GetString("Unknown_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something went wrong with an order.
        /// </summary>
        internal static string Unknown_Subject {
            get {
                return ResourceManager.GetString("Unknown_Subject", resourceCulture);
            }
        }
    }
}
