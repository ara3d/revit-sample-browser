// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.DynamicModelUpdate.CS
{
    /// <summary>
    ///     Updater to automatically move a section in conjunction with the location of a window
    /// </summary>
    public class SectionUpdater : IUpdater
    {
        private Element m_sectionElement; // The view section element to move and rotate
        private ElementId m_sectionId; // The real ViewSection that contains the Origin and ViewDirection

        // private data:

        private readonly UpdaterId m_updaterId;
        private ElementId m_windowId;

        internal SectionUpdater(AddInId addinId)
        {
            m_updaterId = new UpdaterId(addinId, new Guid("FBF3F6B2-4C06-42d4-97C1-D1B4EB593EFF"));
        }

        // The Execute method for the updater
        public void Execute(UpdaterData data)
        {
            try
            {
                var doc = data.GetDocument();
                // iterate through modified elements to find the one we want the section to follow
                foreach (var id in data.GetModifiedElementIds())
                    if (id == m_windowId)
                    {
                        var window = doc.GetElement(m_windowId) as FamilyInstance;
                        var section = doc.GetElement(m_sectionId) as ViewSection;

                        RejustSectionView(doc, window, section);
                    }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Exception", ex.ToString());
            }
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Associative Section Updater";
        }

        public string GetAdditionalInformation()
        {
            return "Automatically moves a section to maintain its position relative to a window";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        // Registers itself with Revit
        internal void Register(Document doc)
        {
            // Register the section updater if the updater is not registered.
            if (!UpdaterRegistry.IsUpdaterRegistered(m_updaterId))
                UpdaterRegistry.RegisterUpdater(this, doc);
        }

        internal void AddTriggerForUpdater(Document doc, List<ElementId> idsToWatch, ElementId sectionId,
            Element sectionElement)
        {
            if (idsToWatch.Count == 0)
                return;

            m_windowId = idsToWatch[0];
            m_sectionId = sectionId;
            m_sectionElement = sectionElement;
            UpdaterRegistry.AddTrigger(m_updaterId, doc, idsToWatch, Element.GetChangeTypeGeometry());
        }

        internal void RejustSectionView(Document doc, Element elem, ViewSection section)
        {
            var position = XYZ.Zero;
            var fOrientation = XYZ.Zero;
            if (elem is FamilyInstance familyInstance)
            {
                if (familyInstance.Location != null && familyInstance.Location is LocationPoint locationPoint)
                {
                    position = locationPoint.Point;
                }

                fOrientation = familyInstance.FacingOrientation;
            }

            var sOrigin = section.Origin;
            var sDirection = section.ViewDirection;

            var fRectOrientation = fOrientation.CrossProduct(XYZ.BasisZ);

            // Rotate the section element
            var angle = fOrientation.AngleTo(sDirection);
            // Need to adjust the rotation angle based on the direction of rotation (not covered by AngleTo)
            var cross = fRectOrientation.CrossProduct(sDirection).Normalize();
            var sign = 1.0;
            if (!cross.IsAlmostEqualTo(XYZ.BasisZ)) sign = -1.0;

            double rotateAngle = 0;
            if (Math.Abs(angle) > 0 && Math.Abs(angle) <= Math.PI / 2.0)
            {
                if (angle < 0)
                    rotateAngle = Math.PI / 2.0 + angle;
                else
                    rotateAngle = Math.PI / 2.0 - angle;
            }
            else if (Math.Abs(angle) > Math.PI / 2.0)
            {
                if (angle < 0)
                    rotateAngle = angle + Math.PI / 2.0;
                else
                    rotateAngle = angle - Math.PI / 2.0;
            }

            rotateAngle *= sign;

            if (Math.Abs(rotateAngle) > 0)
            {
                var axis = Line.CreateBound(sOrigin, sOrigin + XYZ.BasisZ);
                ElementTransformUtils.RotateElement(doc, m_sectionElement.Id, axis, rotateAngle);
            }

            // Regenerate the document
            doc.Regenerate();

            // Move the section element
            var dotF = position.DotProduct(fRectOrientation);
            var dotS = sOrigin.DotProduct(fRectOrientation);
            var moveDot = dotF - dotS;
            var sNewDirection = section.ViewDirection; // Get the new direction after rotation.
            var correction = fRectOrientation.DotProduct(sNewDirection);
            var translationVec = sNewDirection * correction * moveDot;

            if (!translationVec.IsZeroLength())
                ElementTransformUtils.MoveElement(doc, m_sectionElement.Id, translationVec);
        }
    }
}
