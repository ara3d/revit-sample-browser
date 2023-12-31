// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    /// <summary>
    ///     This class is used for window creation
    /// </summary>
    public abstract class WindowCreation
    {
        /// <summary>
        ///     The parameter of Window wizard
        /// </summary>
        public readonly WizardParameter Para;

        /// <summary>
        ///     The constructor of WindowCreation
        /// </summary>
        /// <param name="parameter">WizardParameter</param>
        protected WindowCreation(WizardParameter parameter)
        {
            Para = parameter;
        }

        /// <summary>
        ///     The function is used to create frame
        /// </summary>
        public abstract void CreateFrame();

        /// <summary>
        ///     The function is used to create sash
        /// </summary>
        public abstract void CreateSash();

        /// <summary>
        ///     The function is used to create glass
        /// </summary>
        public abstract void CreateGlass();

        /// <summary>
        ///     The function is used to create material
        /// </summary>
        public abstract void CreateMaterial();

        /// <summary>
        ///     The function is used to combine and build the window family
        /// </summary>
        public abstract void CombineAndBuild();

        /// <summary>
        ///     The function is used to do the whole creation work.
        /// </summary>
        public abstract bool Creation();
    }
}
