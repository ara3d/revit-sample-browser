using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DuctResize sample.</summary>
    internal static partial class Util
    {
        private const BuiltInParameter DuctResizeBipDiameter
            = BuiltInParameter.RBS_CURVE_DIAMETER_PARAM;

        private const BuiltInParameter DuctResizeBipHeight
            = BuiltInParameter.RBS_CURVE_HEIGHT_PARAM;

        private const double DuctResizeTwoInches = 1.0 / 6.0;

        /// <summary>
        ///     Return shape of this duct, retrieved
        ///     from its first or first two connectors.
        /// </summary>
        internal static ConnectorProfileType GetDuctShape(Duct d)
        {
            Debug.Assert(null != d.ConnectorManager,
                "expected a valid connector manager on a duct");

            var cons = d.ConnectorManager.Connectors;

            Debug.Assert(null != cons,
                "expected valid connectors on a duct");

            Debug.Assert(2 <= cons.Size,
                "expected at least two connectors on a duct");

            var shape
                = ConnectorProfileType.Invalid;

            foreach (Connector c in cons)
            {
#if DEBUG
                if (ConnectorProfileType.Invalid != shape)
                {
                    Debug.Assert(shape == c.Shape,
                        "expected same shape on first two duct connectors");
                    break;
                }
#endif // DEBUG

                shape = c.Shape;

                Debug.Assert(ConnectorProfileType.Invalid != shape,
                    "expected valid shape on first two duct connectors");

#if !DEBUG
        break;
#endif // DEBUG
            }

            return shape;
        }

        /// <summary>
        ///     Return dimension for this duct:
        ///     diameter if round, else height.
        /// </summary>
        internal static double GetDuctDim(
            Duct d,
            ConnectorProfileType shape)
        {
            return ConnectorProfileType.Round == shape
                ? d.Diameter
                : d.Height;
        }

        /// <summary>
        ///     Return dimension for this connector:
        ///     diameter if round, else height.
        /// </summary>
        internal static double GetConnectorDim(Connector c)
        {
            var shape = c.Shape;

            return ConnectorProfileType.Round == shape
                ? 2 * c.Radius
                : c.Height;
        }

        /// <summary>
        ///     Resize ducts to ensure that branch ducts are no
        ///     larger than the main duct they are tapping into.
        /// </summary>
        internal static void DuctResize(Document doc)
        {
            var ductCollector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Duct));

            using var transaction = new Transaction(doc);
            if (transaction.Start("Resize Ducts for Taps")
                == TransactionStatus.Started)
            {
                var i = 0;
                foreach (Duct d in ductCollector)
                {
                    var shape = GetDuctShape(d);
                    var dctCnnctrs = d.ConnectorManager.Connectors;

                    var nDCs = dctCnnctrs.Size;
                    if (nDCs < 3)
                    {
                        // do nothing
                    }
                    else
                    {
                        var ductDim = GetDuctDim(d, shape);

                        var largestConnector = 0.0;

                        foreach (Connector c in dctCnnctrs)
                            if (c.ConnectorType.ToString().Equals("End"))
                            {
                                // Do nothing because I am not 
                                // interested in the "End" Connectors
                            }
                            else
                            {
                                var taps = c.AllRefs;

                                var maxTapDim = 0.0;

                                foreach (Connector cd in taps)
                                {
                                    var tapDim = GetConnectorDim(cd);

                                    if (maxTapDim < tapDim) maxTapDim = tapDim;
                                }

                                if (largestConnector < maxTapDim) largestConnector = maxTapDim;
                            }

                        if (largestConnector > ductDim)
                        {
                            var updatedHeight = largestConnector
                                                + DuctResizeTwoInches;

                            var ductHeight
                                = d.get_Parameter(DuctResizeBipHeight)
                                  ?? d.get_Parameter(DuctResizeBipDiameter);

                            var oldHeight = ductHeight.AsDouble();

                            if (!IsEqual(oldHeight, updatedHeight))
                            {
                                ductHeight.Set(updatedHeight);

                                ++i;
                            }
                        }
                    }
                }

                var taskDialog = new TaskDialog(
                    "Resize Ducts");

                TaskDialogCommonButtons buttons;

                if (0 < i)
                {
                    var n = ductCollector.GetElementCount();

                    taskDialog.MainContent = $"{i} out of {n} ducts will be re-sized.\n\nClick [OK] to Commit or [Cancel] to Roll back the transaction.";

                    buttons = TaskDialogCommonButtons.Ok
                              | TaskDialogCommonButtons.Cancel;
                }
                else
                {
                    taskDialog.MainContent
                        = "None of the ducts need to be re-sized.";

                    buttons = TaskDialogCommonButtons.Ok;
                }

                taskDialog.CommonButtons = buttons;

                if (TaskDialogResult.Ok == taskDialog.Show() && 0 < i)
                    if (TransactionStatus.Committed != transaction.Commit())
                        TaskDialog.Show("Failure",
                            "Transaction could not be committed");
            }
        }
    }
}
