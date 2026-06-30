// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
namespace Ara3D.RevitSampleBrowser.RebarFreeForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AddSharedParams : IExternalCommand
    {
        /// <summary>Shared param toggled to force free-form rebar regeneration when the linked curve changes.</summary>
        public static readonly string ParamName = "Updated";

        /// <summary>Stores the linked model curve ElementId as a string (ElementId is 64-bit).</summary>
        public static readonly string CurveIdName = "CurveElementId";

        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                if (doc == null)
                    return Result.Failed;
                using Transaction tran = new(doc, "Add shared param");
                tran.Start();

                // Add Shared parameters:
                //   Update is a simple boolean.
                //   CurveElementId is an ElementId, which is a 64-bit Entity, so stringify it to keep data intact. 
                var paramsAdded = ParameterAccess.AddSharedTestParameter(commandData, ParamName, SpecTypeId.Boolean.YesNo, false);
                paramsAdded &= ParameterAccess.AddSharedTestParameter(commandData, CurveIdName, SpecTypeId.String.Text, true);
                if (paramsAdded)
                {
                    tran.Commit();
                    return Result.Succeeded;
                }

                tran.RollBack();
                return Result.Failed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }
}
