﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MyCC.Core.Resources {
    using System;
    using System.Reflection;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class I18N {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal I18N() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("MyCC.Core.Resources.I18N", typeof(I18N).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string Bittrex {
            get {
                return ResourceManager.GetString("Bittrex", resourceCulture);
            }
        }
        
        internal static string Blockchain {
            get {
                return ResourceManager.GetString("Blockchain", resourceCulture);
            }
        }
        
        internal static string BlockExperts {
            get {
                return ResourceManager.GetString("BlockExperts", resourceCulture);
            }
        }
        
        internal static string CryptoId {
            get {
                return ResourceManager.GetString("CryptoId", resourceCulture);
            }
        }
        
        internal static string Etherchain {
            get {
                return ResourceManager.GetString("Etherchain", resourceCulture);
            }
        }
        
        internal static string LocalStorage {
            get {
                return ResourceManager.GetString("LocalStorage", resourceCulture);
            }
        }
        
        internal static string Address {
            get {
                return ResourceManager.GetString("Address", resourceCulture);
            }
        }
        
        internal static string ManuallyAdded {
            get {
                return ResourceManager.GetString("ManuallyAdded", resourceCulture);
            }
        }
        
        internal static string Blockr {
            get {
                return ResourceManager.GetString("Blockr", resourceCulture);
            }
        }
        
        internal static string Kraken {
            get {
                return ResourceManager.GetString("Kraken", resourceCulture);
            }
        }
        
        internal static string Cryptonator {
            get {
                return ResourceManager.GetString("Cryptonator", resourceCulture);
            }
        }
        
        internal static string Btce {
            get {
                return ResourceManager.GetString("Btce", resourceCulture);
            }
        }
        
        internal static string FixerIo {
            get {
                return ResourceManager.GetString("FixerIo", resourceCulture);
            }
        }
    }
}
