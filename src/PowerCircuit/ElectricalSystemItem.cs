// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    /// <summary>
    ///     An electrical system item contains the name and id of an electrical system.
    ///     The class is used for displaying electrical systems in circuit selecting form.
    /// </summary>
    public class ElectricalSystemItem
    {
        private readonly ElementId m_id;

        private readonly string m_name;

        public ElectricalSystemItem(ElectricalSystem es)
        {
            m_name = es.Name;
            m_id = es.Id;
        }

        public ElementId Id => m_id;

        public string Name => m_name;
    }
}
