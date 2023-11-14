// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.NewRoof.RoofForms.CS
{
    /// <summary>
    ///     The RoofItem is used to display a roof info in the ListView as a ListViewItem.
    /// </summary>
    internal class RoofItem : ListViewItem
    {
        // To store the roof which the RoofItem stands for.

        /// <summary>
        ///     The construct of the RoofItem class.
        /// </summary>
        /// <param name="roof"></param>
        public RoofItem(RoofBase roof) : base(roof.Id.ToString())
        {
            Roof = roof;
            SubItems.Add(roof.Name);

            if (Roof is FootPrintRoof)
            {
                var para = roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                SubItems.Add(LevelConverter.GetLevelByID(para.AsElementId()).Name);
            }
            else if (Roof is ExtrusionRoof)
            {
                var para = roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                SubItems.Add(LevelConverter.GetLevelByID(para.AsElementId()).Name);
            }

            SubItems.Add(roof.RoofType.Name);
        }

        /// <summary>
        ///     Get the roof which the RoofItem stands for.
        /// </summary>
        public RoofBase Roof { get; }

        /// <summary>
        ///     When the roof was edited, then the data of the RoofItem should be updated synchronously.
        /// </summary>
        /// <returns>Update successfully return true, otherwise return false.</returns>
        public bool Update()
        {
            try
            {
                SubItems[1].Text = Roof.Name;

                if (Roof is FootPrintRoof)
                {
                    var para = Roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                    SubItems[2].Text = LevelConverter.GetLevelByID(para.AsElementId()).Name;
                }
                else if (Roof is ExtrusionRoof)
                {
                    var para = Roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                    SubItems[2].Text = LevelConverter.GetLevelByID(para.AsElementId()).Name;
                }

                SubItems[3].Text = Roof.RoofType.Name;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
