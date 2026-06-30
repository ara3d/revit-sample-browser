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
        private readonly UIDocument m_rvtDoc;

        public CreationMgr(UIDocument rvtDoc)
        {
            m_rvtDoc = rvtDoc;
        }

        public FasciaCreator FasciaCreator => field ??= new FasciaCreator(m_rvtDoc);

        public GutterCreator GutterCreator => field ??= new GutterCreator(m_rvtDoc);

        public SlabEdgeCreator SlabEdgeCreator
            => field ??= new SlabEdgeCreator(m_rvtDoc);

        public void Execute()
        {
            using MainForm mainForm = new(this);
            mainForm.ShowDialog();
        }
    }
}
