// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Revit.SDK.Samples.PowerCircuit.CS
{
    /// <summary>
    ///     An electrical system item contains the name and id of an electrical system.
    ///     The class is used for displaying electrical systems in circuit selecting form.
    /// </summary>
    public class ElectricalSystemItem
    {
        /// <summary>
        ///     Id of an electrical system
        /// </summary>
        private readonly ElementId m_id;

        /// <summary>
        ///     Name of an electrical system
        /// </summary>
        private readonly string m_name;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="es"></param>
        public ElectricalSystemItem(ElectricalSystem es)
        {
            m_name = es.Name;
            m_id = es.Id;
        }

        /// <summary>
        ///     Id of an electrical system
        /// </summary>
        public ElementId Id => m_id;

        /// <summary>
        ///     Name of an electrical system
        /// </summary>
        public string Name => m_name;
    }
}
