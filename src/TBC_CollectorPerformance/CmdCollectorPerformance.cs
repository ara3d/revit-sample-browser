#region Header

//
// CmdCollectorPerformance.cs - benchmark Revit 2011 API collector performance
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

#endregion // Namespaces

namespace BuildingCoder
{

    #region Type filter versus anonymous method versus LINQ by Piotr Zurek

    //
    // Compare TypeFilter versus using an
    // anonymous method to filter elements.
    // By Guy Robinson, info@r-e-d.co.nz.
    //
    // Copyright (C) 2008 by Jeremy Tammik,
    // Autodesk Inc. All rights reserved.
    //
    // Updated to the Revit 2011 API and added LINQ filtering.
    // By Piotr Zurek, p.zurek@gmail.com
    //
    //#region Imported Namespaces

    ////.NET common used namespaces
    //using System;
    //using System.Linq;
    //using System.Diagnostics;
    //using System.Collections.Generic;

    ////Revit.NET common used namespaces
    //using Autodesk.Revit.Attributes;
    //using Autodesk.Revit.DB;
    //using Autodesk.Revit.UI;

    //using Application = Autodesk.Revit.ApplicationServices.Application;

    //#endregion

    namespace FilterPerformance
    {

    }

    #endregion // Type filter versus anonymous method versus LINQ by Piotr Zurek

    #region Filter for elements in a specific view having a specific phase



    #endregion // Filter for elements in a specific view having a specific phase

    #region Parameter filter using display name


    #endregion // Parameter filter using display name

    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdCollectorPerformance : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            Util.ListElementsInAssembly(doc);

            //Util.RunBenchmark(doc);

            var wall = Util.SelectSingleElementOfType(
                uidoc, typeof(Wall), "a wall", true);

            Util.GetInstancesIntersectingElement(wall);

            return Result.Succeeded;
        }
    }

    #region YBExporteContext

#if YBExporteContext
  internal class YBExporteContext : IExportContext
  {
    private Document _host_document;
    private IEnumerable<View> _2D_views_that_can_display_elements;

    public YBExporteContext(
      Document document,
      View activeView )
    {
      this._host_document = document;
      this._2D_views_that_can_display_elements
        = YbUtil.FindAllViewsThatCanDisplayElements(
          document );
    }

    /*
      * Lot of code here implementing the 
      * "IExportContext" interface...
      */

    private GeometryElement _get2DRepresentation(
      Element element )
    {
      View view = this._get2DViewForElement( element );
      if( view == null )
        return null;

      Options options = new Options();
      options.View = view;
      return element.get_Geometry( options );
    }

    /// <summary>
    /// Gets any 2D view where the element is displayed
    /// </summary>
    /// <param name="element"></param>
    /// <returns>A 2D view where the element is displayed</returns>
    private View _get2DViewForElement( Element element )
    {
      FilteredElementCollector collector;
      ICollection<ElementId> elements_in_view;

      foreach( View view in
        this._2D_views_that_can_display_elements )
      {
        collector = new FilteredElementCollector(
          this._host_document, view.Id )
            .WhereElementIsNotElementType();

        elements_in_view = collector.ToElementIds();

        if( elements_in_view.Contains( element.Id ) )
          return view;
      }

      return null;
    }
  }

  public static class YbUtil
  {
    public static IEnumerable<View>
      FindAllViewsThatCanDisplayElements(
        Document doc )
    {
      ElementMulticlassFilter filter
        = new ElementMulticlassFilter( new List<Type> { typeof( ViewPlan ) } );

      return new FilteredElementCollector( doc )
        .WherePasses( filter )
        .Cast<View>()
        .Where( v => !v.IsTemplate && v.CanBePrinted );
    }
  }
#endif // YBExporteContext

    #endregion // YBExporteContext
}