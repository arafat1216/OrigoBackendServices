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
    }
}