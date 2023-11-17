// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    public class Application : IExternalApplication
    {
        // class instance
        public static Application ThisApp;
        private ElementId m_currentAppearanceAssetElementId;
        private Material m_currentMaterial;
        private Document m_document;

        // ModelessForm instance
        private AppearanceAssetEditingForm m_myForm;
        private UIApplication m_revit;

        public Result OnShutdown(UIControlledApplication application)
        {
            if (m_myForm != null && !m_myForm.IsDisposed)
            {
                m_myForm.Dispose();
                m_myForm = null;

                // if we've had a dialog, we had subscribed
                application.Idling -= IdlingHandler;
            }

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            m_myForm = null; // no dialog needed yet; the command will bring it
            ThisApp = this; // static access to this application instance

            return Result.Succeeded;
        }

        /// <summary>
        ///     This method creates and shows a modeless dialog, unless it already exists.
        /// </summary>
        /// <remarks>
        ///     The external command invokes this on the end-user's request
        /// </remarks>
        public void ShowForm(UIApplication uiapp)
        {
            m_revit = uiapp;
            m_document = uiapp.ActiveUIDocument.Document;

            // If we do not have a dialog yet, create and show it
            if (m_myForm == null || m_myForm.IsDisposed)
            {
                m_myForm = new AppearanceAssetEditingForm();
                m_myForm.Show();

                // if we have a dialog, we need Idling too
                uiapp.Idling += IdlingHandler;
            }
        }

        /// <summary>
        ///     Compares two colors.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <returns>True if the colors are equal, false otherwise.</returns>
        private bool ColorsEqual(Color color1, Color color2)
        {
            return color1.Red == color2.Red && color1.Green == color2.Green && color1.Blue == color2.Blue;
        }

        private void Log(string msg)
        {
            var dt = DateTime.Now.ToString("u");
            Trace.WriteLine(dt + " " + msg);
        }

        /// <summary>
        ///     Get the painted material from selection.
        /// </summary>
        public void GetPaintedMaterial()
        {
            Reference refer;

            try
            {
                refer = m_revit.ActiveUIDocument.Selection.PickObject(ObjectType.Face,
                    new IsPaintedFaceSelectionFilter(), "Select a painted face for editing.");
            }
            catch (OperationCanceledException)
            {
                // Selection Cancelled.
                return;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                Log(ex.Message);
                return;
            }

            if (refer == null) return;

            var element = m_document.GetElement(refer);
            var face = element.GetGeometryObjectFromReference(refer) as Face;
            var matId = m_document.GetPaintedMaterial(element.Id, face);
            m_currentMaterial = m_document.GetElement(matId) as Material;

            if (m_currentMaterial != null)
            {
                m_currentAppearanceAssetElementId = m_currentMaterial.AppearanceAssetId;

                // Clear selection
                m_revit.ActiveUIDocument.Selection.GetElementIds().Clear();
            }
        }

        /// <summary>
        ///     Check if the selected material supports "tint".
        /// </summary>
        /// <returns>True if the selected material supports "tint" or not.</returns>
        private bool SupportTintColor()
        {
            if (m_currentAppearanceAssetElementId == ElementId.InvalidElementId)
                return false;

            if (!(m_document.GetElement(m_currentAppearanceAssetElementId) is AppearanceAssetElement assetElem))
                return false;

            var asset = assetElem.GetRenderingAsset();
            if (!(asset.FindByName("common_Tint_color") is AssetPropertyDoubleArray4d tintColorProp))
                return false;

            // If the material supports tint but it is not enabled, it will be enabled first with a value (255 255 255)
            if (asset.FindByName("common_Tint_color") is AssetPropertyBoolean tintToggleProp && !tintToggleProp.Value) EnableTintColor();

            return true;
        }

        /// <summary>
        ///     Enable tint color.
        /// </summary>
        private void EnableTintColor()
        {
            using (var transaction = new Transaction(m_document, "Enable tint color"))
            {
                transaction.Start();

                using (var editScope = new AppearanceAssetEditScope(m_document))
                {
                    var editableAsset = editScope.Start(m_currentAppearanceAssetElementId);

                    //  If the material supports tint but it is not enabled, it will be enabled first with a value (255 255 255)
                    var tintToggleProp = editableAsset.FindByName("common_Tint_color") as AssetPropertyBoolean;
                    var tintColorProp = editableAsset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;

                    tintToggleProp.Value = true;
                    tintColorProp.SetValueAsColor(new Color(255, 255, 255));

                    editScope.Commit(true);
                }

                transaction.Commit();
            }
        }

        /// <summary>
        ///     Check if the button Lighter is enabled.
        /// </summary>
        /// <returns>True if the material can be made lighter or not.</returns>
        public bool IsLighterEnabled()
        {
            if (!SupportTintColor())
                return false;

            if (!(m_document.GetElement(m_currentAppearanceAssetElementId) is AppearanceAssetElement assetElem))
                return false;

            var asset = assetElem.GetRenderingAsset();
            var tintColorProp = asset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;
            var tintColor = tintColorProp.GetValueAsColor();
            var white = new Color(255, 255, 255);
            return !ColorsEqual(tintColor, white);
        }

        /// <summary>
        ///     Check if the button Darker is enabled.
        /// </summary>
        /// <returns>True if the material can be made darker or not.</returns>
        public bool IsDarkerEnabled()
        {
            if (!SupportTintColor())
                return false;

            if (!(m_document.GetElement(m_currentAppearanceAssetElementId) is AppearanceAssetElement assetElem))
                return false;

            var asset = assetElem.GetRenderingAsset();
            var tintColorProp = asset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;
            var tintColor = tintColorProp.GetValueAsColor();
            var black = new Color(0, 0, 0);
            return !ColorsEqual(tintColor, black);
        }

        /// <summary>
        ///     A handler for the Idling event.
        /// </summary>
        /// <remarks>
        ///     We keep the handler very simple. First we check
        ///     if we still have the dialog. If not, we unsubscribe from Idling,
        ///     for we no longer need it and it makes Revit speedier.
        ///     If we do have the dialog around, we check if it has a request ready
        ///     and process it accordingly.
        /// </remarks>
        public void IdlingHandler(object sender, IdlingEventArgs args)
        {
            var uiapp = sender as UIApplication;

            if (m_myForm.IsDisposed)
            {
                uiapp.Idling -= IdlingHandler;
                return;
            }

            // dialog still exists
            // fetch the request from the dialog           
            var requestId = m_myForm.Request.Take();
            if (requestId != RequestId.None)
                try
                {
                    // we take the request, if any was made,
                    // and pass it on to the request executor     
                    RequestHandler.Execute(this, requestId);
                }
                finally
                {
                    // The dialog may be in its waiting state;
                    // make sure we wake it up even if we get an exception.    
                    m_myForm.EnableButtons(IsLighterEnabled(), IsDarkerEnabled());
                }
        }

        /// <summary>
        ///     Limit value to 0-255
        /// </summary>
        /// <returns>True if the material can be made darker or not.</returns>
        private int LimitValue(int value)
        {
            return value < 0 ? 0 : value > 255 ? 255 : value;
        }

        /// <summary>
        ///     The main material-modification subroutine - called from every request
        /// </summary>
        /// <remarks>
        ///     It searches the current selection for all doors
        ///     and if it finds any it applies the requested operation to them
        /// </remarks>
        /// <param name="text">Caption of the transaction for the operation.</param>
        /// <param name="lighter">Increase the tint color property or not.</param>
        public void ModifySelectedMaterial(string text, bool lighter)
        {
            // Since we'll modify the document, we need a transaction
            // It's best if a transaction is scoped by a 'using' block
            using (var trans = new Transaction(m_document))
            {
                // The name of the transaction was given as an argument  
                if (trans.Start(text) == TransactionStatus.Started)
                {
                    // apply the requested operation to every door    
                    EditMaterialTintColorProperty(lighter);

                    trans.Commit();
                }
            }
        }

        /// <summary>
        ///     Edit tint color property.
        /// </summary>
        /// <param name="lighter">Increase the tint color property or not.</param>
        public void EditMaterialTintColorProperty(bool lighter)
        {
            using (var editScope = new AppearanceAssetEditScope(m_document))
            {
                var editableAsset = editScope.Start(m_currentAppearanceAssetElementId);

                var metalColorProp = editableAsset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;

                var color = metalColorProp.GetValueAsColor();
                var red = color.Red;
                var green = color.Green;
                var blue = color.Blue;

                // Increment factor  (value related to 255)
                var factor = 25;

                if (lighter)
                {
                    red = (byte)LimitValue(red + factor);
                    green = (byte)LimitValue(green + factor);
                    blue = (byte)LimitValue(blue + factor);
                }
                else
                {
                    red = (byte)LimitValue(red - factor);
                    green = (byte)LimitValue(green - factor);
                    blue = (byte)LimitValue(blue - factor);
                }

                if (metalColorProp.IsValidValue(color))
                    metalColorProp.SetValueAsColor(new Color(red, green, blue));

                editScope.Commit(true);
            }
        }

        /// <summary>
        ///     Custom filter for selection.
        /// </summary>
        private class IsPaintedFaceSelectionFilter : ISelectionFilter
        {
            private Document m_selectedDocument;

            public bool AllowElement(Element element)
            {
                m_selectedDocument = element.Document;
                return true;
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                if (m_selectedDocument == null)
                    throw new Exception("AllowElement was never called for this reference...");

                var element = m_selectedDocument.GetElement(refer);
                var face = element.GetGeometryObjectFromReference(refer) as Face;

                return m_selectedDocument.IsPainted(element.Id, face);
            }
        }
    }
}
