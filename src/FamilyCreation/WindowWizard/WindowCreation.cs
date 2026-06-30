// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public abstract class WindowCreation
    {
        public readonly WizardParameter Para;

        protected WindowCreation(WizardParameter parameter)
        {
            Para = parameter;
        }

        public abstract void CreateFrame();

        public abstract void CreateSash();

        public abstract void CreateGlass();

        public abstract void CreateMaterial();

        public abstract void CombineAndBuild();

        public abstract bool Creation();
    }
}
