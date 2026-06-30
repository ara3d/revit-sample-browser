// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
namespace Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS
{
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

        public void ModifySelectedMaterial(string text, bool lighter)
        {
            using Transaction trans = new(m_document);
            if (trans.Start(text) == TransactionStatus.Started)
            {
                EditMaterialTintColorProperty(lighter);
                trans.Commit();
            }
        }

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
                return;
            }
            catch (Exception ex)
            {
                EventLoggingHelper.Log(ex.Message);
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
                m_revit.ActiveUIDocument.Selection.GetElementIds().Clear();
            }
        }

        private bool SupportTintColor()
        {
            if (m_currentAppearanceAssetElementId == ElementId.InvalidElementId)
                return false;

            if (m_document.GetElement(m_currentAppearanceAssetElementId) is not AppearanceAssetElement assetElem)
                return false;

            var asset = assetElem.GetRenderingAsset();
            if (asset.FindByName("common_Tint_color") is not AssetPropertyDoubleArray4d tintColorProp)
                return false;

            // If the material supports tint but it is not enabled, it will be enabled first with a value (255 255 255)
            if (asset.FindByName("common_Tint_color") is AssetPropertyBoolean tintToggleProp && !tintToggleProp.Value) EnableTintColor();

            return true;
        }

        private void EnableTintColor()
        {
            using Transaction transaction = new(m_document, "Enable tint color");
            transaction.Start();

            using (AppearanceAssetEditScope editScope = new(m_document))
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

        public bool IsLighterEnabled()
        {
            if (!SupportTintColor())
                return false;

            if (m_document.GetElement(m_currentAppearanceAssetElementId) is not AppearanceAssetElement assetElem)
                return false;

            var asset = assetElem.GetRenderingAsset();
            var tintColorProp = asset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;
            var tintColor = tintColorProp.GetValueAsColor();
            Color white = new(255, 255, 255);
            return !BitmapHelper.ColorsEqual(tintColor, white);
        }

        public bool IsDarkerEnabled()
        {
            if (!SupportTintColor())
                return false;

            if (m_document.GetElement(m_currentAppearanceAssetElementId) is not AppearanceAssetElement assetElem)
                return false;

            var asset = assetElem.GetRenderingAsset();
            var tintColorProp = asset.FindByName("common_Tint_color") as AssetPropertyDoubleArray4d;
            var tintColor = tintColorProp.GetValueAsColor();
            Color black = new(0, 0, 0);
            return !BitmapHelper.ColorsEqual(tintColor, black);
        }

        // A handler for the Idling event.
        // We keep the handler very simple. First we check
        // if we still have the dialog. If not, we unsubscribe from Idling,
        // for we no longer need it and it makes Revit speedier.
        // If we do have the dialog around, we check if it has a request ready
        // and process it accordingly.
        public void IdlingHandler(object sender, IdlingEventArgs args)
        {
            var uiapp = sender as UIApplication;

            if (m_myForm.IsDisposed)
            {
                uiapp.Idling -= IdlingHandler;
                return;
            }

            var requestId = m_myForm.Request.Take();
            if (requestId != RequestId.None)
                try
                {
                    // and pass it on to the request executor     
                    RequestHandler.Execute(this, requestId);
                }
                finally
                {
                    // make sure we wake it up even if we get an exception.    
                    m_myForm.EnableButtons(IsLighterEnabled(), IsDarkerEnabled());
                }
        }

        public void EditMaterialTintColorProperty(bool lighter)
        {
            using AppearanceAssetEditScope editScope = new(m_document);
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
                red = (byte)BitmapHelper.LimitValue(red + factor);
                green = (byte)BitmapHelper.LimitValue(green + factor);
                blue = (byte)BitmapHelper.LimitValue(blue + factor);
            }
            else
            {
                red = (byte)BitmapHelper.LimitValue(red - factor);
                green = (byte)BitmapHelper.LimitValue(green - factor);
                blue = (byte)BitmapHelper.LimitValue(blue - factor);
            }

            if (metalColorProp.IsValidValue(color))
                metalColorProp.SetValueAsColor(new Color(red, green, blue));

            editScope.Commit(true);
        }
    }
}
