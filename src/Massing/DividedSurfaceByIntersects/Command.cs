// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Massing.DividedSurfaceByIntersects.CS
{
    /// <summary>
    ///     the entry point of the sample
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     The active Revit document
        /// </summary>
        private Document m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // store the active Revit document
            m_document = commandData.Application.ActiveUIDocument.Document;

            var ds = GetDividedSurface();
            if (null == ds)
            {
                message = "Open the family file from the sample folder first.";
                return Result.Failed;
            }

            var planes = GetPlanes();
            var lines = GetLines();

            var act = new Transaction(m_document);
            act.Start("AddRemoveIntersects");
            try
            {
                // step 1: divide the surface with reference planes and levels
                foreach (var id in planes)
                {
                    if (ds.CanBeIntersectionElement(id))
                        ds.AddIntersectionElement(id);
                }

                // step 2: remove all the reference planes and level intersection elements
                IEnumerable<ElementId> intersects = ds.GetAllIntersectionElements();

                foreach (var id in intersects)
                {
                    ds.RemoveIntersectionElement(id);
                }

                // step 3: divide the surface with model lines instead
                foreach (var id in lines)
                {
                    if (ds.CanBeIntersectionElement(id))
                        ds.AddIntersectionElement(id);
                }
            }
            catch (Exception)
            {
                act.RollBack();
            }
            finally
            {
                act.Commit();
            }

            return Result.Succeeded;
        }

        private DividedSurface GetDividedSurface()
        {
            return m_document.GetElement(new ElementId(31519L)) as DividedSurface;
        }

        private IEnumerable<ElementId> GetPlanes()
        {
            // 1027, 1071 & 1072 are ids of the reference planes and levels drawn in the family file
            yield return new ElementId(1027L);
            yield return new ElementId(1071L);
            yield return new ElementId(1072L);
        }

        private IEnumerable<ElementId> GetLines()
        {
            // the "31xxx" numberic values are ids of the model lines drawn in the family file
            yield return new ElementId(31170L);
            yield return new ElementId(31206L);
            yield return new ElementId(31321L);
            yield return new ElementId(31343L);
            yield return new ElementId(31377L);
            yield return new ElementId(31395L);
        }

        /// <summary>
        ///     Get element by its Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eid"></param>
        /// <returns></returns>
        public T GetElement<T>(long eid) where T : Element
        {
            return m_document.GetElement(new ElementId(eid)) as T;
        }
    }
}
