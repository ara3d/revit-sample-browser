using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FilePath = Ara3D.Utils.FilePath;

namespace Ara3D.Bowerbird.RevitSamples
{
    /// <summary>
    /// Save the current view as a PNG in the temp folder, and then opens
    /// it using the default registered application. 
    /// </summary>
    public class CommandSaveCurrentViewAsPng : NamedCommand
    {
        public override string Name => "Save to PNG";

        public override void Execute(object arg)
        {
            var doc = (arg as UIApplication)?.ActiveUIDocument?.Document;
            if (doc == null) return;
            var output = PathUtil.CreateTempFile().ChangeExtension("png");
            ExportCurrentViewToPng(doc, output);
            output.ShellExecute();
        }

        public static FilePath ExportCurrentViewToPng(Document doc, FilePath filePath)
        {
            ImageExportOptions img = new()
            {
                ZoomType = ZoomFitType.FitToPage,
                PixelSize = 1024,
                ImageResolution = ImageResolution.DPI_600,
                FitDirection = FitDirectionType.Horizontal,
                ExportRange = ExportRange.CurrentView,
                HLRandWFViewsFileType = ImageFileType.PNG,
                FilePath = filePath,
                ShadowViewsFileType = ImageFileType.PNG
            };
            doc.ExportImage(img);
            return filePath;
        }
    }
}