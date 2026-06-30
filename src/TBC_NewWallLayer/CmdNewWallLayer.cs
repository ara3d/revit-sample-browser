#region Header

//
// CmdNewWallLayer.cs - create a new compound wall layer.
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Compound wall layers became editable from Revit 2012 onward (previously read-only).</summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewWallLayer : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

#if _2011
      //
      // code for the Revit 2011 API:
      //
      Debug.Assert( false,
        "Currently, no new wall layer can be created, because"
        + "there is no creation method available for it." );

      foreach( WallType wallType in doc.WallTypes )
      {
        if( 0 < wallType.CompoundStructure.Layers.Size )
        {
          CompoundStructureLayer oldLayer
            = wallType.CompoundStructure.Layers.get_Item( 0 );

          WallType newWallType
            = wallType.Duplicate( "NewWallType" ) as WallType;

          CompoundStructure structure
            = newWallType.CompoundStructure;

          CompoundStructureLayerArray layers
            = structure.Layers;


          // from here on, nothing works, as expected:
          // in the Revir 2010 API, we could call the constructor
          // even though it is for internal use only.
          // in 2011, it is not possible to call it either.

          CompoundStructureLayer newLayer = null;
          //  = new CompoundStructureLayer(); // for internal use only

          newLayer.DeckProfile = oldLayer.DeckProfile;
          //newLayer.DeckUsage = oldLayer.DeckUsage; // read-only
          //newLayer.Function = oldLayer.Function; // read-only
          newLayer.Material = oldLayer.Material;
          newLayer.Thickness = oldLayer.Thickness;
          newLayer.Variable = oldLayer.Variable;
          layers.Append( newLayer );

        }
      }
#endif // _2011

            using Transaction t = new(doc);
            t.Start("Create New Wall Layer");


            var wallTypes
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType));

            foreach (WallType wallType in wallTypes)
                if (0 < wallType.GetCompoundStructure().GetLayers().Count)
                {
                    var oldLayer
                        = wallType.GetCompoundStructure().GetLayers()[0];

                    var newWallType
                        = wallType.Duplicate("NewWallType") as WallType;

                    var structure
                        = newWallType.GetCompoundStructure();

                    var layers
                        = structure.GetLayers();


                    var width = 0.1;
                    var function = oldLayer.Function;
                    var materialId = oldLayer.MaterialId;

                    CompoundStructureLayer newLayer
                        = new(width, function, materialId);

                    layers.Add(newLayer);
                    structure.SetLayers(layers);
                    newWallType.SetCompoundStructure(structure);
                }

            t.Commit();

            return Result.Succeeded;
        }
    }
}