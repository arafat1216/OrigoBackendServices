﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AssetServices.Resources {
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
    internal class Asset {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Asset() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AssetServices.Resources.Asset", typeof(Asset).Assembly);
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
        ///&lt;/br&gt;
        ///An asset was successfully reassigned from your department.
        ///&lt;/br&gt;
        ///[View Asset]({{AssetLink}}).
        /// </summary>
        internal static string ReassignedToUser {
            get {
                return ResourceManager.GetString("ReassignedToUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello!
        ///&lt;/br&gt;
        ///An asset was successfully unassigned from your department.
        ///&lt;/br&gt;
        ///This means you are no longer responsible for this asset, and it is therefore no longer visible to you.
        ///&lt;/br&gt;.
        /// </summary>
        internal static string UnassignedFromManager {
            get {
                return ResourceManager.GetString("UnassignedFromManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {{FirstName}}!
        ///&lt;/br&gt;
        ///An asset was successfully unassigned from you.
        ///&lt;/br&gt;
        ///This means you are no longer responsible for this asset, and it is therefore no longer visible to you.
        ///&lt;/br&gt;.
        /// </summary>
        internal static string UnassignedFromUser {
            get {
                return ResourceManager.GetString("UnassignedFromUser", resourceCulture);
            }
        }
    }
}