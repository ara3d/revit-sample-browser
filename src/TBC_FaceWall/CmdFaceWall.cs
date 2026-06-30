#region Header

//
// CmdFaceWall.cs - demonstrate FaceWall.Create
//
// Create and insert a conceptual mass family instance, 
// then create sloped walls on all its faces.
//
// Copyright (C) 2014-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    #region Automatic Walls

    // For 13642689 [Mathematical Translations]
    // https://forums.autodesk.com/t5/revit-api-forum/mathematical-translations/m-p/7580510


    #endregion // Automatic Walls

    [Transaction(TransactionMode.Manual)]
    internal class CmdFaceWall : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            Util.CreateFaceWalls(doc);

            return Result.Succeeded;
        }

        #region CreateFaceWallsAndMassFloors

        // By Harry Mattison, Boost Your BIM,
        // Automating the Building Maker workflow
        // https://boostyourbim.wordpress.com/2014/02/11/automating-the-building-maker-workflow/
        // Face Wall and Mass Floor creation with the Revit API
        // https://youtu.be/nHWen2_lN6U

        public void CreateFaceWallsAndMassFloors(UIDocument uidoc)
        {
            Util.CreateFaceWallsAndMassFloors(uidoc);
        }

        #endregion // CreateFaceWallsAndMassFloors

        #region Original code

#if REVIT_2012_CODE
    public static void SlopedWallTest(
      ExternalCommandData revit )
    {
      Document massDoc = revit.Application.Application.NewFamilyDocument(
          @"C:\ProgramData\Autodesk\RAC 2012\Family Templates\English_I\Conceptual Mass\Mass.rft" );

      Transaction transaction = new Transaction( massDoc );
      transaction.SetName( "TEST" );
      transaction.Start();

      ExternalCommandData cdata = revit;
      Autodesk.Revit.ApplicationServices.Application app = revit.Application.Application;
      app = revit.Application.Application;

      // Create one profile
      ReferenceArray ref_ar = new ReferenceArray();

      Autodesk.Revit.DB.XYZ ptA = new XYZ( 0, 0, 0 );
      XYZ ptB = new XYZ( 0, 30, 0 );
      ModelCurve modelcurve = MakeLine( revit.Application, ptA, ptB, massDoc );
      ref_ar.Append( modelcurve.GeometryCurve.Reference );

      ptA = new XYZ( 0, 30, 0 );
      ptB = new XYZ( 2, 30, 0 );
      modelcurve = MakeLine( revit.Application, ptA, ptB, massDoc );
      ref_ar.Append( modelcurve.GeometryCurve.Reference );

      ptA = new XYZ( 2, 30, 0 );
      ptB = new XYZ( 2, 0, 0 );
      modelcurve = MakeLine( revit.Application, ptA, ptB, massDoc );
      ref_ar.Append( modelcurve.GeometryCurve.Reference );

      ptA = new XYZ( 2, 0, 0 );
      ptB = new XYZ( 0, 0, 0 );
      modelcurve = MakeLine( revit.Application, ptA, ptB, massDoc );
      ref_ar.Append( modelcurve.GeometryCurve.Reference );

      // The extrusion form direction
      XYZ direction = new XYZ( -6, 0, 50 );
      Form form = massDoc.FamilyCreate.NewExtrusionForm( true, ref_ar, direction );
      transaction.Commit();

      if( File.Exists( @"C:\TestFamily.rfa" ) )
        File.Delete( @"C:\TestFamily.rfa" );

      massDoc.SaveAs( @"C:\TestFamily.rfa" );

      if( !revit.Application.ActiveUIDocument.Document.LoadFamily( @"C:\TestFamily.rfa" ) )
        throw new Exception( "DID NOT LOAD FAMILY" );

      Family family = null;
      foreach( Element el in new FilteredElementCollector(
          revit.Application.ActiveUIDocument.Document ).WhereElementIsNotElementType().ToElements() )
      {
        if( el is Family )
        {
          if( ( (Family) el ).Name.ToUpper().Trim().StartsWith( "TEST" ) )
            family = (Family) el;
        }
      }

      FamilySymbol fs = null;
      foreach( FamilySymbol sym in family.Symbols )
        fs = sym;

      // Create a family instance.
      revit.Application.ActiveUIDocument.Document.Create.NewFamilyInstance(
          new XYZ( 0, 0, 0 ), fs, revit.Application.ActiveUIDocument.Document.ActiveView.Level,
          StructuralType.NonStructural );

      WallType wallType = null;
      foreach( WallType wt in revit.Application.ActiveUIDocument.Document.WallTypes )
      {
        if( FaceWall.IsWallTypeValidForFaceWall( revit.Application.ActiveUIDocument.Document, wt.Id ) )
        {
          wallType = wt;
          break;
        }
      }

      foreach( Element el in new FilteredElementCollector(
          revit.Application.ActiveUIDocument.Document ).WhereElementIsNotElementType().ToElements() )
      {
        if( el is FamilyInstance )
        {
          if( ( (FamilyInstance) el ).Symbol.Family.Name.ToUpper().StartsWith( "TEST" ) )
          {
            Options options = revit.Application.Application.Create.NewGeometryOptions();
            options.ComputeReferences = true;
            options.View = revit.Application.ActiveUIDocument.Document.ActiveView;
            GeometryElement geoel = el.get_Geometry( options );

            // Attempt to create a slopped wall from the geometry.
            for( int i = 0; i < geoel.Objects.Size; i++ )
            {
              if( geoel.Objects.get_Item( i ) is Solid )
              {
                Solid solid = (Solid) geoel.Objects.get_Item( i );
                for( int j = 0; j < solid.Faces.Size; j++ )
                {
                  try
                  {
                    if( solid.Faces.get_Item( i ).Reference != null )
                    {
                      FaceWall.Create( revit.Application.ActiveUIDocument.Document,
                          wallType.Id, WallLocationLine.CoreCenterline,
                          solid.Faces.get_Item( i ).Reference );
                    }
                  }
                  catch( System.Exception e )
                  {
                    System.Windows.Forms.MessageBox.Show( e.Message );
                  }
                }
              }
            }
          }
        }
      }
    }

    public static ModelCurve MakeLine( UIApplication app, XYZ ptA, XYZ ptB, Document doc )
    {
      // Create plane by the points
      Line line = app.Application.Create.NewLine( ptA, ptB, true );
      XYZ norm = ptA.CrossProduct( ptB );
      if( norm.GetLength() == 0 ) norm = XYZ.BasisZ;
      Plane plane = app.Application.Create.NewPlane( norm, ptB );
      SketchPlane skplane = doc.FamilyCreate.NewSketchPlane( plane );
      // Create line here
      ModelCurve modelcurve = doc.FamilyCreate.NewModelCurve( line, skplane );
      return modelcurve;
    }
#endif // REVIT_2012_CODE

        #endregion // Original code
    }
}
