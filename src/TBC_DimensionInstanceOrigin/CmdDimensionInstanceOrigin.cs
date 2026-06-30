#region Header

//
// CmdDimensionInstanceOrigin.cs - create dimensioning between the origins of family instances
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
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    #region Scott Conover sample code for SPR #201483

    // SPR #201483 [API wish: access reference plane 
    // in family instance and retrieve dimensioning 
    // reference to it]

    #endregion // Scott's sample code from SPR #201483

    [Transaction(TransactionMode.Manual)]
    internal class CmdDimensionInstanceOrigin : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            JtPairPicker<FamilyInstance> picker
                = new(uidoc);

            var rc = picker.Pick();

            switch (rc)
            {
                case Result.Failed:
                    message = "We need at least two "
                              + "FamilyInstance elements in the model.";
                    break;
                case Result.Succeeded:
                    {
                        var a = picker.Selected;

                        var pts = new XYZ[2];
                        var refs = new Reference[2];

                        pts[0] = (a[0].Location as LocationPoint).Point;
                        pts[1] = (a[1].Location as LocationPoint).Point;

                        refs[0] = Util.GetFamilyInstancePointReference(a[0]);
                        refs[1] = Util.GetFamilyInstancePointReference(a[1]);

                        Util.CreateDimensionElement(doc.ActiveView,
                                pts[0], refs[0], pts[1], refs[1]);
                        break;
                    }
            }

            return rc;
        }

        #region Dimensioning wall corners

#if THIS_CODE_COMPILATION_FAILS
    // https://forums.autodesk.com/t5/revit-api-forum/dimension-between-walls-corners-using-revit-s-api/m-p/7228752
    static double _offset;

    List<Reference> GetWallOpenings( Wall wall, View3D view )
    {
      Document doc = wall.Document;
      Level level = doc.GetElement( wall.LevelId ) as Level;
      double elevation = level.Elevation;
      Curve c = ( wall.Location as LocationCurve ).Curve;
      XYZ wallOrigin = c.GetEndPoint( 0 );
      XYZ wallEndPoint = c.GetEndPoint( 1 );
      XYZ wallDirection = wallEndPoint - wallOrigin;
      double wallLength = wallDirection.GetLength();
      wallDirection = wallDirection.Normalize();

      UV offsetOut = _offset * new UV( 
        wallDirection.X, wallDirection.Y );

      XYZ rayStart = new XYZ( 
        wallOrigin.X - offsetOut.U, 
        wallOrigin.Y - offsetOut.V, 
        elevation + _offset );

      ReferenceIntersector intersector
        = new ReferenceIntersector( 
          wall.Id, FindReferenceTarget.Face, view );

      IList<ReferenceWithContext> refs
        = intersector.Find( rayStart, wallDirection );

      List<Reference> faceReferenceList
        = new List<Reference>( refs
          .Where<ReferenceWithContext>( r => IsSurface(
            r.GetReference() ) )
          .Where<ReferenceWithContext>( r => r.Proximity
            < wallLength + _offset + _offset )
          .Select<ReferenceWithContext, Reference>( r
            => r.GetReference() ) );

      return faceReferenceList;
    }

    public void test( UIDocument uidoc )
    {
      Document doc = uidoc.Document;

      ReferenceArray refs = new ReferenceArray();

      Reference myRef = uidoc.Selection.PickObject( 
        ObjectType.Element, 
        new MySelectionFilter( "Walls" ), 
        "Select a wall" );

      Wall wall = doc.GetElement( myRef ) as Wall;

      // Creates an element e from the selected object 
      // reference -- this will be the wall element
      Element e = doc.GetElement( myRef );

      // Creates a selection filter to dump objects 
      // in for later selection
      ICollection<ElementId> selSet = new List<ElementId>();

      // Gets the bounding box of the selected wall 
      // element picked above
      BoundingBoxXYZ bb = e.get_BoundingBox( doc.ActiveView );

      // adds a buffer to the bounding box to ensure 
      // all elements are contained within the box
      XYZ buffer = new XYZ( 0.1, 0.1, 0.1 );

      // creates an ouline based on the boundingbox 
      // corners of the panel and adds the buffer
      Outline outline = new Outline( 
        bb.Min - buffer, bb.Max + buffer );

      // filters the selection by the bounding box of the selected object
      // the "true" statement inverts the selection and selects all other objects
      BoundingBoxIsInsideFilter bbfilter
        = new BoundingBoxIsInsideFilter( outline, false );

      ICollection<BuiltInCategory> bcat
        = new List<BuiltInCategory>();

      //creates a new filtered element collector that 
      // filters by the active view settings
      FilteredElementCollector collector
        = new FilteredElementCollector( 
          doc, doc.ActiveView.Id );

      //collects all objects that pass through the 
      // requirements of the bbfilter
      collector.WherePasses( bbfilter );

      //add all levels and grids to filter -- these 
      // are filtered out by the viewtemplate, but 
      // are nice to have
      bcat.Add( BuiltInCategory.OST_StructConnections );

      //create new multi category filter
      ElementMulticategoryFilter multiCatFilter
        = new ElementMulticategoryFilter( bcat );

      //create new filtered element collector, add the 
      // passing levels and grids, then remove them 
      // from the selection
      foreach( Element el in collector.WherePasses( 
        multiCatFilter ) )
      {
        if( el.Name.Equals( "EMBEDS" ) )
        {
          selSet.Add( el.Id );
        }
      }

      XYZ[] pts = new XYZ[99];

      //View3D view = doc.ActiveView as View3D;
      View3D view = Get3dView( doc );

      // THIS IS WHERE IT RETURNS THE WALL OPENING REFERENCES.  
      // HOWEVER THEY ONLY ARE ABLE TO BE USED FOR DIMENSIONS 
      // IF THE OPENING IS CREATED USING A FAMILY SUCH AS A 
      // WINDOW OR DOOR. OPENING BY FACE/WALL DOES NOT WORK, 
      // EVEN THOUGH IT RETURNS PROPER REFERENCES

      List<Reference> openings = GetWallOpenings( e as Wall, view );

      foreach( Reference reference in openings )
      {
        refs.Append( reference );
      }

      TaskDialog.Show( "REFERE", refs.Size.ToString() );

      Curve wallLocation = ( wall.Location as LocationCurve ).Curve;

      int i = 0;

      foreach( ElementId ele in selSet )
      {
        FamilyInstance fi = doc.GetElement( ele ) as FamilyInstance;
        Reference reference
          = ScottWilsonVoodooMagic.GetSpecialFamilyReference( 
            fi, ScottWilsonVoodooMagic.SpecialReferenceType.CenterLR, 
            doc );

        refs.Append( reference );

        pts[i] = ( fi.Location as LocationPoint ).Point;
        i++;
      }

      XYZ offset = new XYZ( 0, 0, 4 );

      Line line = Line.CreateBound( 
        pts[0] + offset, pts[1] + offset );

      using( Transaction t = new Transaction( doc ) )
      {
        t.Start( "dimension embeds" );

        Dimension dim = doc.Create.NewDimension( doc.ActiveView, line, refs );

        t.Commit();
      }
    }
#endif // THIS_CODE_COMPILATION_FAILS

        #endregion // Dimensioning wall corners
    }
}