//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
// 

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