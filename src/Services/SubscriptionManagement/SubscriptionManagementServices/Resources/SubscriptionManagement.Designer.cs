﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SubscriptionManagementServices.Resources {
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
    internal class SubscriptionManagement {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SubscriptionManagement() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SubscriptionManagementServices.Resources.SubscriptionManagement", typeof(SubscriptionManagement).Assembly);
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
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///---
        ///#### Mobile Number
        ///{{MobileNumber}}
        ///
        ///#### Operator
        ///{{OperatorName}}
        ///
        ///#### SIM Card Number
        ///{{SimCardNumber}}
        ///
        ///#### SIM Card Type
        ///### Order Type: {{OrderType}}
        ///---
        ///#### Mobile Number
        ///{{MobileNumber}}
        ///
        ///#### Operator
        ///{{OperatorName}}
        ///
        ///#### SIM Card Number
        ///{{SimCardNumber}}
        ///
        ///#### SIM Card Type
        ///{{SimCardType}}.
        /// </summary>
        internal static string ActivateSim {
            get {
                return ResourceManager.GetString("ActivateSim", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///---
        ///#### Mobile Number
        ///{{MobileNumber}}
        ///#### Operator
        ///{{OperatorName}}
        ///#### Date Of Termination
        ///{{DateOfTermination}}.
        /// </summary>
        internal static string CancelSubscription {
            get {
                return ResourceManager.GetString("CancelSubscription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///---
        ///#### Mobile Number
        ///{{MobileNumber}}
        ///#### Operator
        ///{{OperatorName}}
        ///#### Product Name
        ///{{ProductName}}
        ///#### PackageName
        ///{{PackageName}}.
        /// </summary>
        internal static string ChangeSubscription {
            get {
                return ResourceManager.GetString("ChangeSubscription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///---
        ///
        ///#### Organization
        ///**Name:** {{BusinessSubscription.Name}}
        ///
        ///**Organization Number:** {{BusinessSubscription.OrganizationNumber}}
        ///
        ///**Address:**
        ///{{BusinessSubscription.Address}}
        ///{{BusinessSubscription.PostalCode}} {{BusinessSubscription.PostalPlace}}
        ///{{BusinessSubscription.Country}}
        ///
        ///#### User
        ///**Name:** {{PrivateSubscription.FirstName}} {{PrivateSubscription.LastName}}
        ///
        ///**Born:** {{PrivateSubscription.BirthDate}}
        ///
        ///**Address:**
        ///{{PrivateSubscription.Address} [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string NewSubscription {
            get {
                return ResourceManager.GetString("NewSubscription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///---
        ///#### User
        ///**Name:**  {{PrivateSubscription.FirstName}} {{PrivateSubscription.LastName}}
        ///
        ///**Born:**  {{PrivateSubscription.BirthDate}}
        ///
        ///**Address:**  {{PrivateSubscription.Address}}, {{PrivateSubscription.PostalCode}}, {{PrivateSubscription.PostalPlace}}, {{PrivateSubscription.Country}}
        ///
        ///**Email:**  {{PrivateSubscription.Email}}
        ///
        ///**MobileNumber:** {{MobileNumber}}
        ///
        ///#### Product
        ///**Operator Name:** {{OperatorName}}
        ///
        ///**Product Name:** {{SubscriptionProductName}} [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransferToBusiness {
            get {
                return ResourceManager.GetString("TransferToBusiness", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ### Order Type: {{OrderType}}
        ///
        ///---
        ///
        ///#### User
        ///
        ///**Name:**  {{UserInfo.FirstName}} {{UserInfo.LastName}}
        ///
        ///**Born:**  {{UserInfo.BirthDate}}
        ///
        ///**Address:**  {{UserInfo.Address}}, {{UserInfo.PostalCode}}, {{UserInfo.PostalPlace}}, {{UserInfo.Country}}
        ///
        ///**Email:**  {{UserInfo.Email}}
        ///
        ///#### Mobile Number
        ///{{MobileNumber}}
        ///
        ///#### Operator Name
        ///{{OperatorName}}
        ///
        ///#### New Subscription
        ///{{NewSubscription}}
        ///
        ///#### Transfer Date
        ///{{OrderExecutionDate}}.
        /// </summary>
        internal static string TransferToPrivate {
            get {
                return ResourceManager.GetString("TransferToPrivate", resourceCulture);
            }
        }
    }
}