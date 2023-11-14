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

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Revit.SDK.Samples.NewHostedSweep.CS
{
    /// <summary>
    ///     This class is intent to provide a model dialog in property grid control.
    /// </summary>
    internal class EdgeFormUITypeEditor : UITypeEditor
    {
        /// <summary>
        ///     Return the Modal style.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Show a form to add or remove edges from hosted sweep, and also can change the
        ///     type of hosted sweep.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context,
            IServiceProvider provider, object value)
        {
            var winSrv = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            var creationData = value as CreationData;
            creationData.BackUp();
            using (var form = new EdgeFetchForm(creationData))
            {
                if (winSrv.ShowDialog(form) == DialogResult.OK)
                    creationData.Update();
                else
                    creationData.Restore();
            }

            return creationData;
        }
    }
}