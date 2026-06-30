// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    public class EdgeFormUiTypeEditor : UITypeEditor
    {
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
            using (EdgeFetchForm form = new(creationData))
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
