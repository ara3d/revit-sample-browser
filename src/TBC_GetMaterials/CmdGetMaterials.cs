#region Header

//
// CmdGetMaterials.cs - determine element materials
// by iterating over its geometry faces
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion // Namespaces

namespace BuildingCoder
{
    #region FindTextureBitmapPaths

    internal class FindTextureBitmapPathsWrapper
    {
        private readonly string[] _targetMaterialNames =
        {
            // A standard Revit material, with 
            // textures in standard paths. 
            "Brick, Common",

            // A material with a single image from 
            // another non-material library path
            "Local Path Material"
        };

        private void FindTextureBitmapPaths(Document doc)
        {
            // Find materials
            FilteredElementCollector fec
                = new(doc);

            fec.OfClass(typeof(Material));

            var targetMaterials
                = fec.Cast<Material>().Where(mtl =>
                    _targetMaterialNames.Contains(mtl.Name));

            foreach (var material in targetMaterials)
            {
                // Get appearance asset for read
                var appearanceAssetId = material
                    .AppearanceAssetId;

                var appearanceAssetElem
                    = doc.GetElement(appearanceAssetId)
                        as AppearanceAssetElement;

                var asset = appearanceAssetElem
                    .GetRenderingAsset();

                // Walk through all first level assets to find 
                // connected Bitmap properties.  Note: it is 
                // possible to have multilevel connected 
                // properties with Bitmaps in the leaf nodes.  
                // So this would need to be recursive.

                var size = asset.Size;
                for (var assetIdx = 0; assetIdx < size; assetIdx++)
                {
                    //AssetProperty aProperty = asset[assetIdx]; // 2018
                    var aProperty = asset.Get(assetIdx); // 2019

                    if (aProperty.NumberOfConnectedProperties < 1)
                        continue;

                    // Find first connected property.  
                    // Should work for all current (2018) schemas.  
                    // Safer code would loop through all connected
                    // properties based on the number provided.

                    var connectedAsset = aProperty
                        .GetConnectedProperty(0) as Asset;

                    // We are only checking for bitmap connected assets. 

                    if (connectedAsset.Name == "UnifiedBitmapSchema")
                    {
                        // This line is 2018.1 & up because of the 
                        // property reference to UnifiedBitmap
                        // .UnifiedbitmapBitmap.  In earlier versions,
                        // you can still reference the string name 
                        // instead: "unifiedbitmap_Bitmap"

                        //AssetPropertyString path = connectedAsset[ // 2018
                        //  UnifiedBitmap.UnifiedbitmapBitmap]
                        //    as AssetPropertyString;

                        var path = connectedAsset // 2019
                                .FindByName(UnifiedBitmap.UnifiedbitmapBitmap)
                            as AssetPropertyString;

                        // This will be a relative path to the 
                        // built -in materials folder, addiitonal 
                        // render appearance folder, or an 
                        // absolute path.

                        TaskDialog.Show("Connected bitmap",
                            string.Format("{0} from {2}: {1}",
                                aProperty.Name, path.Value,
                                connectedAsset.LibraryName));
                    }
                }
            }
        }
    }

    #endregion // FindTextureBitmapPaths

    #region Victor sample code

    public class ElementComparer : IEqualityComparer<Element>
    {
        public bool Equals(Element x, Element y)
        {
            return x != null && y != null && x.Id.Value == y.Id.Value && x.GetType() == y.GetType() && x.Document.Equals(y.Document);
        }

        public int GetHashCode(Element obj)
        {
            return obj.UniqueId.GetHashCode();
        }
    }

    public class MaterialComparer : IEqualityComparer<Material>
    {
        public bool Equals(Material x, Material y)
        {
            return x.UniqueId.Equals(y.UniqueId);
        }

        public int GetHashCode(Material obj)
        {
            return obj.UniqueId.GetHashCode();
        }
    }

    #endregion // Victor sample code

    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdGetMaterials : IExternalCommand
    {
        #region List Material Asset Sub-Texture
        internal class AssetPropertyPropertyDescriptor : PropertyDescriptor
        {
            public AssetPropertyPropertyDescriptor(AssetProperty assetProperty)
                : base(assetProperty.Name, Array.Empty<Attribute>())
            {
                AssetProperty = assetProperty;
            }

            #region Properties
            public AssetProperty AssetProperty { get; }

            #endregion
            private static double[] GetSystemArray(DoubleArray doubleArray)
            {
                var values = new double[doubleArray.Size];
                var index = 0;
                foreach (double value in doubleArray) values[index++] = value;
                return values;
            }

            private static string GetSystemArrayAsString(DoubleArray doubleArray)
            {
                var values = GetSystemArray(doubleArray);

                //String result = "";
                //foreach( double d in values )
                //{
                //  result += d;
                //  result += ",";
                //}

                //return result;

                return string.Join(",",
                    values.Select(
                        x => x.ToString()));
            }

            #region Fields
            /// m
            private Type _valueType;
            private object _value;

            #endregion

            #region override Properties
            public override bool IsReadOnly => true;
            public override Type ComponentType => AssetProperty.GetType();
            public override Type PropertyType => _valueType;

            #endregion

            #region override methods
            public override bool Equals(object obj)
            {
                return obj is AssetPropertyPropertyDescriptor other && other.AssetProperty.Equals(AssetProperty);
            }
            public override int GetHashCode()
            {
                return AssetProperty.GetHashCode();
            }
            public override void ResetValue(object component)
            {
            }
            public override bool CanResetValue(object component)
            {
                return false;
            }
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
            public override object GetValue(object component)
            {
                var typeAndValue = GetTypeAndValue(AssetProperty, 0);
                _value = typeAndValue.Item2;
                _valueType = typeAndValue.Item1;

                return _value;
            }

            private static Tuple<Type, object> GetTypeAndValue(AssetProperty assetProperty, int level)
            {
                object theValue;
                Type valueType;
                //For each AssetProperty, it has different type and value
                //must deal with it separately
                try
                {
                    if (assetProperty is AssetPropertyBoolean boolean)
                    {
                        valueType = typeof(AssetPropertyBoolean);
                        theValue = boolean.Value;
                    }
                    else if (assetProperty is AssetPropertyDistance distance)
                    {
                        valueType = typeof(AssetPropertyDistance);
                        theValue = distance.Value;
                    }
                    else if (assetProperty is AssetPropertyDouble d)
                    {
                        valueType = typeof(AssetPropertyDouble);
                        theValue = d.Value;
                    }
                    else if (assetProperty is AssetPropertyDoubleArray2d array2d)
                    {
                        //Default, it is supported by PropertyGrid to display Double []
                        //Try to convert DoubleArray to Double []
                        valueType = typeof(AssetPropertyDoubleArray2d);
                        theValue = GetSystemArrayAsString(array2d.Value);
                    }
                    else if (assetProperty is AssetPropertyDoubleArray3d array3d)
                    {
                        valueType = typeof(AssetPropertyDoubleArray3d);
                        //theValue = GetSystemArrayAsString( property.Value ); // 2017
                        theValue = Util.DoubleArrayString(array3d.GetValueAsDoubles()); // 2018
                    }
                    else if (assetProperty is AssetPropertyDoubleArray4d array4d)
                    {
                        valueType = typeof(AssetPropertyDoubleArray4d);
                        //theValue = GetSystemArrayAsString( property.Value ); // 2017
                        theValue = Util.DoubleArrayString(array4d.GetValueAsDoubles()); // 2018
                    }
                    else if (assetProperty is AssetPropertyDoubleMatrix44 matrix44)
                    {
                        valueType = typeof(AssetPropertyDoubleMatrix44);
                        theValue = GetSystemArrayAsString(matrix44.Value);
                    }
                    else if (assetProperty is AssetPropertyEnum @enum)
                    {
                        valueType = typeof(AssetPropertyEnum);
                        theValue = @enum.Value;
                    }
                    else if (assetProperty is AssetPropertyFloat f)
                    {
                        valueType = typeof(AssetPropertyFloat);
                        theValue = f.Value;
                    }
                    else if (assetProperty is AssetPropertyInteger integer)
                    {
                        valueType = typeof(AssetPropertyInteger);
                        theValue = integer.Value;
                    }
                    else if (assetProperty is AssetPropertyReference reference)
                    {
                        valueType = typeof(AssetPropertyReference);
                        theValue = "REFERENCE"; //property.Type;
                    }
                    else if (assetProperty is AssetPropertyString s)
                    {
                        valueType = typeof(AssetPropertyString);
                        theValue = s.Value;
                    }
                    else if (assetProperty is AssetPropertyTime property)
                    {
                        valueType = typeof(AssetPropertyTime);
                        theValue = property.Value;
                    }
                    else
                    {
                        valueType = typeof(string);
                        theValue = $"Unprocessed asset type: {assetProperty.GetType().Name}";
                    }

                    if (assetProperty.NumberOfConnectedProperties > 0)
                    {
                        var result = "";
                        result = theValue.ToString();

                        TaskDialog.Show("Connected properties found", $"{assetProperty.Name}: {assetProperty.NumberOfConnectedProperties}");
                        var properties = assetProperty.GetAllConnectedProperties();

                        foreach (var property in properties)
                            if (property is Asset asset)
                            {
                                // Nested?
                                var size = asset.Size;
                                for (var i = 0; i < size; i++)
                                {
                                    //AssetProperty subproperty = asset[i]; // 2018
                                    var subproperty = asset.Get(i); // 2019
                                    var valueAndType = GetTypeAndValue(subproperty, level + 1);
                                    var indent = "";
                                    if (level > 0)
                                        for (var iLevel = 1; iLevel <= level; iLevel++)
                                            indent += "   ";
                                    result += $"\n {indent}- connected: name: {subproperty.Name} | type: {valueAndType.Item1.Name} | value: {valueAndType.Item2}";
                                }
                            }

                        theValue = result;
                    }
                }
                catch
                {
                    return null;
                }

                return new Tuple<Type, object>(valueType, theValue);
            }
            public override void SetValue(object component, object value)
            {
            }

            #endregion
        }
        public class RenderAppearanceDescriptor : ICustomTypeDescriptor
        {
            #region Constructors
            public RenderAppearanceDescriptor(Asset asset)
            {
                _asset = asset;
                GetAssetProperties();
            }

            #endregion

            #region Fields
            private readonly Asset _asset;
            private PropertyDescriptorCollection _propertyDescriptors;

            #endregion

            #region Methods

            #region ICustomTypeDescriptor Members
            public AttributeCollection GetAttributes()
            {
                return TypeDescriptor.GetAttributes(_asset, false);
            }
            public string GetClassName()
            {
                return TypeDescriptor.GetClassName(_asset, false);
            }
            public string GetComponentName()
            {
                return TypeDescriptor.GetComponentName(_asset, false);
            }
            public TypeConverter GetConverter()
            {
                return TypeDescriptor.GetConverter(_asset, false);
            }
            public EventDescriptor GetDefaultEvent()
            {
                return TypeDescriptor.GetDefaultEvent(_asset, false);
            }
            public PropertyDescriptor GetDefaultProperty()
            {
                return TypeDescriptor.GetDefaultProperty(_asset, false);
            }
            public object GetEditor(Type editorBaseType)
            {
                return TypeDescriptor.GetEditor(_asset, editorBaseType, false);
            }
            public EventDescriptorCollection GetEvents(Attribute[] attributes)
            {
                return TypeDescriptor.GetEvents(_asset, attributes, false);
            }
            public EventDescriptorCollection GetEvents()
            {
                return TypeDescriptor.GetEvents(_asset, false);
            }
            public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return _propertyDescriptors;
            }
            public PropertyDescriptorCollection GetProperties()
            {
                return _propertyDescriptors;
            }
            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                return _asset;
            }

            #endregion
            private void GetAssetProperties()
            {
                if (null == _propertyDescriptors)
                    _propertyDescriptors = new PropertyDescriptorCollection(Array.Empty<AssetPropertyPropertyDescriptor>());
                else
                    return;

                //For each AssetProperty in Asset, create an AssetPropertyPropertyDescriptor.
                //It means that each AssetProperty will be a property of Asset
                for (var index = 0; index < _asset.Size; index++)
                {
                    //AssetProperty assetProperty = m_asset[index]; // 2018
                    var assetProperty = _asset.Get(index); // 2019

                    if (null != assetProperty)
                    {
                        AssetPropertyPropertyDescriptor assetPropertyPropertyDescriptor = new(assetProperty);
                        _propertyDescriptors.Add(assetPropertyPropertyDescriptor);
                    }
                }
            }

            #endregion
        }

        public void ShowMaterialInfo(Document doc)
        {
            // Find material
            FilteredElementCollector fec = new(doc);
            fec.OfClass(typeof(Material));

            var materialName = "Checker"; // "Copper";//"Prism - Glass - Textured"; // "Parking Stripe"; // "Copper";
            // "Carpet (1)";// "Prism - Glass - Textured";// "Parking Stripe"; // "Prism 1";// "Brick, Common" ;// "Acoustic Ceiling Tile 24 x 48";  // "Aluminum"
            var mat = fec.Cast<Material>().First(m => m.Name == materialName);

            var appearanceAssetId = mat.AppearanceAssetId;

            var appearanceAsset = doc.GetElement(appearanceAssetId) as AppearanceAssetElement;

            var renderingAsset = appearanceAsset.GetRenderingAsset();

            RenderAppearanceDescriptor rad
                = new(renderingAsset);

            var collection = rad.GetProperties();

            TaskDialog.Show("Total properties", $"Properties found: {collection.Count}");

            var s = ": Material Asset Properties";

            TaskDialog dlg = new(s)
            {
                MainInstruction = mat.Name + s
            };

            s = string.Empty;

            List<PropertyDescriptor> orderableCollection = [.. collection.Cast<PropertyDescriptor>()];

            foreach (var descr in
                orderableCollection.OrderBy(pd => pd.Name).Cast<AssetPropertyPropertyDescriptor>())
            {
                var value = descr.GetValue(rad);

                s += $"\nname: {descr.Name} | type: {descr.PropertyType.Name} | value: {value}";
            }

            dlg.MainContent = s;
            dlg.Show();
        }

        //public void ListAllAssets(UIApplication uiapp)
        //{
        //  AssetSet assets = uiapp.Application.get_Assets( // Revit 2019
        //    AssetType.Appearance);
        //  TaskDialog dlg = new TaskDialog("Assets");
        //  String assetLabel = "";
        //  foreach (Asset asset in assets)
        //  {
        //    String libraryName = asset.LibraryName;
        //    //AssetPropertyString uiname = asset["UIName"] as AssetPropertyString; // 2018
        //    AssetPropertyString uiname = asset.FindByName("UIName") as AssetPropertyString; // 2019
        //    //AssetPropertyString baseSchema = asset["BaseSchema"] as AssetPropertyString; // 2018
        //    AssetPropertyString baseSchema = asset.FindByName("BaseSchema") as AssetPropertyString; // 2019
        //    assetLabel += libraryName + " | " + uiname.Value + " | " + baseSchema.Value;
        //    assetLabel += "\n";
        //  }
        //  dlg.MainInstruction = assetLabel;
        //  dlg.Show();
        //}

        #endregion // List Material Asset Sub-Texture

        #region Victor sample code

        private class ElementMaterial
        {
            public ElementMaterial(Element element, Material material)
            {
                Element = element ?? throw new ArgumentNullException("element");
                Material = material ?? throw new ArgumentNullException("material");
            }

            public Material Material { get; }

            public Element Element { get; }
        }

#if BEFORE_REVIT_2013
    void Victor( Document document )
    {
      var material = document
        .Settings
        .Materials
        .OfType<Material>()
        .FirstOrDefault( m => m.Name.Equals( "Concrete" ) );

      List<Element> elements = null;

      var elementsWithMaterials =
        ( from el in elements
          from Material m in el.Materials
          select new ElementMaterial( el, m ) )
        .ToList();

      var groupedElementsInMaterial
        = elementsWithMaterials
          .GroupBy( x => x.Material, new MaterialComparer() )
          .OrderBy( x => x.Key.Name );
    }
#endif // BEFORE_REVIT_2013

        #endregion // Victor sample code

#if BEFORE_REVIT_2013
    List<Material> GetSortedMaterials( Document doc )
    {
      Materials doc_materials
        = doc.Settings.Materials;

      int n = doc_materials.Size;

      List<Material> materials_sorted
        = new List<Material>( n );

      foreach( Material m in doc_materials )
      {
        if( m != null )
        {
          materials_sorted.Add( m );
        }
      }
      materials_sorted.Sort( 
        delegate( Material m1, Material m2 )
        {
          return m1.Name.CompareTo( m2.Name );
        }
      );
      return materials_sorted;
    }
#endif // BEFORE_REVIT_2013

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var ids = sel.GetElementIds();
            var opt = app.Application.Create.NewGeometryOptions();
            Material mat;
            var msg = string.Empty;
            int i, n;

            foreach (var id in ids)
            {
                var e = doc.GetElement(id);

                // For 0310_ensure_material.htm:

                if (e is FamilyInstance instance)
                {
                    mat = Util.GetFamilyInstanceMaterial(doc, instance);

                    Util.InfoMsg(
                        $"Family instance element material: {(null == mat ? "<null>" : mat.Name)}");
                }

                var geo = e.get_Geometry(opt);

                // If you are not interested in duplicate
                // materials, you can define a class that
                // overloads the Add method to only insert
                // a new entry if its value is not already
                // present in the list, instead of using
                // the standard List<> class:

                var materials = Util.GetMaterialsFromGeometry(doc, geo);

                msg += $"\n{Util.ElementDescription(e)}";

                n = materials.Count;

                if (0 == n)
                {
                    msg += " has no materials.";
                }
                else
                {
                    i = 0;

                    msg += $" has {n} material{Util.PluralSuffix(n)}:";

                    foreach (var s in materials)
                        msg += $"\n  {i++} {s}";
                }
            }

            if (0 == msg.Length) msg = "Please select some elements.";

            Util.InfoMsg(msg);

            return Result.Succeeded;
        }

        #region Access All Material Asset Properties

        private void GetElementMaterialInfo(Document doc)
        {
            var collector
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(Material));

            try
            {
                foreach (Material material in collector)
                    if (material.Name.Equals("Air"))
                    {
                        var appearanceElement
                            = doc.GetElement(material.AppearanceAssetId)
                                as AppearanceAssetElement;

                        var appearanceAsset = appearanceElement
                            .GetRenderingAsset();

                        List<AssetProperty> assetProperties
                            = [];

                        var physicalPropSet
                            = doc.GetElement(material.StructuralAssetId)
                                as PropertySetElement;

                        var thermalPropSet
                            = doc.GetElement(material.ThermalAssetId)
                                as PropertySetElement;

                        var thermalAsset = thermalPropSet
                            .GetThermalAsset();

                        var physicalAsset = physicalPropSet
                            .GetStructuralAsset();

                        ICollection<Parameter> physicalParameters
                            = physicalPropSet.GetOrderedParameters();

                        ICollection<Parameter> thermalParameters
                            = thermalPropSet.GetOrderedParameters();

                        // Appearance Asset

                        for (var i = 0; i < appearanceAsset.Size; i++)
                        {
                            var property = appearanceAsset[i];
                            assetProperties.Add(property);
                        }

                        foreach (var assetProp in assetProperties)
                        {
                            var type = assetProp.GetType();
                            object assetPropValue = null;
                            var prop = type.GetProperty("Value");
                            if (prop != null
                                && prop.GetIndexParameters().Length == 0)
                                assetPropValue = prop.GetValue(assetProp);
                        }

                        // Physical (Structural) Asset

                        foreach (var p in physicalParameters)
                        {
                            // Work with parameters here
                            // The only parameter not in the orderedParameters 
                            // that is needed is the Asset name, which you 
                            // can get by 'physicalAsset.Name'.
                        }

                        // Thermal Asset

                        foreach (var p in thermalParameters)
                        {
                            //Work with parameters here
                            //The only parameter not in the orderedParameters 
                            // that is needed is the Asset name, shich you 
                            // can get by 'thermalAsset.Name'.
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion // Access All Material Asset Properties

        #region Set material appearance asset keyword property

        //I have an issue setting a string value to the material appearance asset keyword property.
        //In one material, it can be set as expected, but another material returns an error saying, "The input value is invalid for this AssetPropertyString property.\r\nParameter name: value".
        //I found the blog article and JIRA ticket REVIT-170824 which explains that the keyword property on the Identity tab is not exposed yet.
        //https://thebuildingcoder.typepad.com/blog/2019/11/material-physical-and-thermal-assets.html#4
        //https://jira.autodesk.com/browse/REVIT-170824
        //I expect the "keyword" property on the appearance tab to accept a string value.
        //In addition, I can see some error message in the journal file.
        //Is it possible to set the "keyword" property of the appearance asset?
        private void SetMaterialAppearanceAssetKeywordProperty(
            AppearanceAssetElement assetElem,
            string new_keyword)
        {
            var doc = assetElem.Document;

            //FilteredElementCollector materialCollector
            //  = new FilteredElementCollector( doc )
            //    .OfCategory( BuiltInCategory.OST_Materials )
            //    .OfClass( typeof( Material ) );
            //Material material = null;
            //foreach( Element e in materialCollector )
            //{
            //  if( e.Name == "HC_CB" )
            //  {
            //    material = e as Material;
            //  }
            //}
            //AppearanceAssetElement assetElem 
            //  = doc.GetElement( material.AppearanceAssetId ) 
            //    as AppearanceAssetElement;

            using Transaction tx = new(doc);
            tx.Start("Transaction Set Keyword");
            using (AppearanceAssetEditScope editScope
                = new(assetElem.Document))
            {
                var editableAsset = editScope.Start(assetElem.Id);

                try
                {
                    var parameter = editableAsset.FindByName("keyword");
                    if (parameter is AssetPropertyString propKeyword)
                    {
                        if (string.IsNullOrEmpty(propKeyword.Value))
                        {
                            propKeyword.Value = new_keyword;
                        }
                        else
                        {
                            if (!propKeyword.Value.Contains(new_keyword))
                            {
                                var val = $"{propKeyword.Value}: {new_keyword}";
                                propKeyword.Value = val;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                editScope.Commit(true);
            }

            tx.Commit();
        }

        #endregion // Set material appearance asset keyword property
    }

    #region Victor sample code 2

#if BEFORE_REVIT_2013


  public static class DocumentExtensions
  {
    public static IEnumerable<Material> GetMaterials(
      this Document doc )
    {
      FilteredElementCollector collector
        = new FilteredElementCollector( doc );

      return collector
        .OfClass( typeof( Material ) )
        .OfType<Material>();
    }

    public static Material GetMaterialByName(
      this Materials materials,
      string materialName )
    {
      return materials
        .OfType<Material>()
        .FirstOrDefault(
          m => m.Name.Equals( materialName ) );
    }
  }
#endif // BEFORE_REVIT_2013

    #endregion // Victor sample code 2
}