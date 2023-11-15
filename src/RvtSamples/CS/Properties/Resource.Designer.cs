// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace RvtSamples.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RvtSamples.Properties.Resource", typeof(Resource).Assembly);
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
        ///   Looks up a localized string similar to Contains samples related to annotations such as notes, detail curves, dimensions and tags..
        /// </summary>
        internal static string Annotation {
            get {
                return ResourceManager.GetString("Annotation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to the ExternalCommand framework, transactions, and event registration..
        /// </summary>
        internal static string Basics {
            get {
                return ResourceManager.GetString("Basics", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to import, export, or print of Revit documents..
        /// </summary>
        internal static string DataExchange {
            get {
                return ResourceManager.GetString("DataExchange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to Revit basic elements such as walls, roofs, floors, levels, grids..
        /// </summary>
        internal static string Elements {
            get {
                return ResourceManager.GetString("Elements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to family creation and family instances..
        /// </summary>
        internal static string Families {
            get {
                return ResourceManager.GetString("Families", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to read and analysis of element geometry including curves and reference planes..
        /// </summary>
        internal static string Geometry {
            get {
                return ResourceManager.GetString("Geometry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to materials and units..
        /// </summary>
        internal static string Materials {
            get {
                return ResourceManager.GetString("Materials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains MEP-specific samples..
        /// </summary>
        internal static string MEP {
            get {
                return ResourceManager.GetString("MEP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to creation, modification and read of element and shared parameters..
        /// </summary>
        internal static string Parameters {
            get {
                return ResourceManager.GetString("Parameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to rooms, spaces and areas..
        /// </summary>
        internal static string RoomsAndSpaces {
            get {
                return ResourceManager.GetString("RoomsAndSpaces", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains structure-specific samples..
        /// </summary>
        internal static string Structure {
            get {
                return ResourceManager.GetString("Structure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contains samples related to view creation and view properties..
        /// </summary>
        internal static string Views {
            get {
                return ResourceManager.GetString("Views", resourceCulture);
            }
        }
    }
}
