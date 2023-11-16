// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators
{
    /// <summary>
    ///     This is the manager of all hosted sweep creators, it contains all the creators
    ///     and each creator can create the corresponding hosted sweep. Its "Execute"
    ///     method will show the main dialog for user to create hosted sweeps.
    /// </summary>
    public class CreationMgr
    {
        /// <summary>
        ///     Creator for Fascia.
        /// </summary>
        private FasciaCreator m_fasciaCreator;

        /// <summary>
        ///     Creator for Gutter.
        /// </summary>
        private GutterCreator m_gutterCreator;

        /// <summary>
        ///     Revit active document.
        /// </summary>
        private readonly UIDocument m_rvtDoc;

        /// <summary>
        ///     Creator for SlabEdge.
        /// </summary>
        private SlabEdgeCreator m_slabEdgeCreator;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="rvtDoc">Revit active document</param>
        public CreationMgr(UIDocument rvtDoc)
        {
            m_rvtDoc = rvtDoc;
        }

        /// <summary>
        ///     Gets Fascia creator.
        /// </summary>
        public FasciaCreator FasciaCreator => m_fasciaCreator ?? (m_fasciaCreator = new FasciaCreator(m_rvtDoc));

        /// <summary>
        ///     Gets Gutter creator.
        /// </summary>
        public GutterCreator GutterCreator => m_gutterCreator ?? (m_gutterCreator = new GutterCreator(m_rvtDoc));

        /// <summary>
        ///     Gets SlabEdge creator.
        /// </summary>
        public SlabEdgeCreator SlabEdgeCreator 
            => m_slabEdgeCreator ?? (m_slabEdgeCreator = new SlabEdgeCreator(m_rvtDoc));

        /// <summary>
        ///     Show the main form, it is the UI entry.
        /// </summary>
        public void Execute()
        {
            using (var mainForm = new MainForm(this))
            {
                mainForm.ShowDialog();
            }
        }
    }
}
