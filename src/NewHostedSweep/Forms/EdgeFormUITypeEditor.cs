// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    /// <summary>
    ///     This class is intent to provide a model dialog in property grid control.
    /// </summary>
    public class EdgeFormUiTypeEditor : UITypeEditor
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
