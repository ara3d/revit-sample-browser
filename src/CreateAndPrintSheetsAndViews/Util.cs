// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    class Util
    {
        public static string ElementDescription(Element e)
        {
            if (null == e) return "<null>";

            var fi = e as FamilyInstance;

            var typeName = e.GetType().Name;

            var categoryName = null == e.Category
                ? string.Empty
                : $"{e.Category.Name} ";

            var familyName = null == fi
                ? string.Empty
                : $"{fi.Symbol.Family.Name} ";

            var symbolName = null == fi
                             || e.Name.Equals(fi.Symbol.Name)
                ? string.Empty
                : $"{fi.Symbol.Name} ";

            return $"{typeName} {categoryName}{familyName}{symbolName}<{e.Id.Value} {e.Name}>";
        }

        private const string _caption = "CreateAndPrintSheetsAndViews";

        public static void InfoMsg2(
            string instruction,
            string content)
        {
            Debug.WriteLine($"{instruction}\r\n{content}");
            var d = new TaskDialog(_caption)
            {
                MainInstruction = instruction,
                MainContent = content
            };
            d.Show();
        }

        public static void InfoMsg3(
            string instruction,
            IList<string> content)
        {
            var s = string.Join("\r\n", content);
            Debug.WriteLine($"{instruction}\r\n{s}");
            var d = new TaskDialog(_caption)
            {
                MainInstruction = instruction,
                MainContent = s
            };
            d.Show();
        }

        public static bool AskYesNoQuestion(string question)
        {
            TaskDialog taskDialog = new("Please answer Yes or No")
            {
                MainContent = question
            };
            var buttons
                = TaskDialogCommonButtons.Yes
                  | TaskDialogCommonButtons.No;
            taskDialog.CommonButtons = buttons;
            var result = taskDialog.Show();
            return result == TaskDialogResult.Yes;
        }

        public static string GetProductCode(Element e)
        {
            const BuiltInParameter _bip_product_code
                = BuiltInParameter.FABRICATION_PRODUCT_CODE;
            var p = e.get_Parameter(_bip_product_code);
            return (null != p)
                ? p.AsString()
                : null;
        }
    }
}
