// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;

using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class GroupVisibilityHelper
    {
        public static void ShowAllAttachedDetailGroups(Group modelGroup, Document doc, View view)
        {
            using Transaction transaction = new(doc, "ShowAllAttachedDetailGroups");
            transaction.Start("Show All Attached Detail Groups");
            modelGroup.ShowAllAttachedDetailGroups(view);
            transaction.Commit();
        }

        public static void HideAllAttachedDetailGroups(Group modelGroup, Document doc, View view)
        {
            using Transaction transaction = new(doc, "HideAllAttachedDetailGroups");
            transaction.Start("Hide All Attached Detail Groups");
            modelGroup.HideAllAttachedDetailGroups(view);
            transaction.Commit();
        }

    }
}