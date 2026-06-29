// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms
{
    /// <summary>
    ///     The RoofItem is used to display a roof info in the ListView as a ListViewItem.
    /// </summary>
    public class RoofItem : ListViewItem
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

            switch (Roof)
            {
                case FootPrintRoof _:
                {
                    var para = roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                    SubItems.Add(LevelConverter.GetLevelById(para.AsElementId()).Name);
                    break;
                }
                case ExtrusionRoof _:
                {
                    var para = roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                    SubItems.Add(LevelConverter.GetLevelById(para.AsElementId()).Name);
                    break;
                }
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

                switch (Roof)
                {
                    case FootPrintRoof _:
                    {
                        var para = Roof.get_Parameter(BuiltInParameter.ROOF_BASE_LEVEL_PARAM);
                        SubItems[2].Text = LevelConverter.GetLevelById(para.AsElementId()).Name;
                        break;
                    }
                    case ExtrusionRoof _:
                    {
                        var para = Roof.get_Parameter(BuiltInParameter.ROOF_CONSTRAINT_LEVEL_PARAM);
                        SubItems[2].Text = LevelConverter.GetLevelById(para.AsElementId()).Name;
                        break;
                    }
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
