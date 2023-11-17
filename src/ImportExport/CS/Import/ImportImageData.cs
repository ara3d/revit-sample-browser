// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Import
{
    /// <summary>
    ///     Data class which stores the information for importing image format
    /// </summary>
    public class ImportImageData : ImportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="importFormat">Format to import</param>
        public ImportImageData(ExternalCommandData commandData, ImportFormat importFormat)
            : base(commandData, importFormat)
        {
            Filter =
                "All Image Files (*.bmp, *.gif, *.jpg, *.jpeg, *.pdf, *.png, *.tif)|*.bmp;*.gif;*.jpg;*.jpeg;*.pdf;*.png;*.tif";
            Title = "Import Image";
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Import()
        {
            using (var t = new Transaction(ActiveDoc))
            {
                t.SetName("Import");
                t.Start();

                // Step 1: Create an ImageType 
                //ImageTypeOptions specifies the source of the image, and how to create the ImageType.
                // It can be used to specify:
                //   - The image file.  Either as a local file path, or as an ExternalResourceRef
                //   - Whether to store the local file path as an absolute path or a relative path.
                //   - Whether to create an Import or a Link image.
                // In addition, if the source is a PDF file, then ImageTypeOptions can be used to specify:
                //   - which page from the PDF to use
                //   - the resolution (in dots-per-inch) at which to rasterize the PDF
                // For other image types the page number should be 1 (the default),
                // and the resolution is only used to determine the size of the image.

                var typeOptions = new ImageTypeOptions(ImportFileFullName, true, ImageTypeSource.Import);
                var imageType = ImageType.Create(ActiveDoc, typeOptions);

                // Step 2: Create an ImageInstance, but only if the active view is able to contain images.
                var view = CommandData.Application.ActiveUIDocument.Document.ActiveView;
                if (ImageInstance.IsValidView(view))
                {
                    // ImagePlacementOptions
                    var placementOptions = new ImagePlacementOptions();
                    placementOptions.PlacementPoint = BoxPlacement.TopLeft;
                    placementOptions.Location = new XYZ(1, 1, 1);

                    ImageInstance.Create(ActiveDoc, view, imageType.Id, placementOptions);
                }

                t.Commit();
            }

            return true;
        }
    }
}
