// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;

namespace Revit.SDK.Samples.UIAPI.CS
{
    public enum ApplicationAvailablity
    {
        Always,
        ArchitectureDiscipline,
        StructuralAnalysis,
        MEP
    }

    public class ApplicationOptions
    {
        private static ApplicationOptions s_options;
        private ExternalApp m_eApplication;

        private ApplicationOptions()
        {
            Availability = ApplicationAvailablity.Always;
        }

        public ApplicationAvailablity Availability { get; set; }


        public static void Initialize(ExternalApp application)
        {
            s_options = new ApplicationOptions
            {
                m_eApplication = application
            };
        }

        public static ApplicationOptions Get()
        {
            if (s_options == null)
                throw new Exception("Static options was not initialized");

            return s_options;
        }
    }
}
