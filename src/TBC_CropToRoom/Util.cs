using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    internal static partial class Util
    {
        private static int _cropToRoomIndex = -1;

        internal static int BumpRoomIndex(int room_count)
        {
            ++_cropToRoomIndex;

            if (_cropToRoomIndex >= room_count) _cropToRoomIndex = 0;
            return _cropToRoomIndex;
        }

        internal static void CropToRoomSectionBox(UIDocument uidoc)
        {
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            var Min_X = double.MaxValue;
            var Min_Y = double.MaxValue;
            var Min_Z = double.MaxValue;

            var Max_X = Min_X;
            var Max_Y = Min_Y;
            var Max_Z = Min_Z;

            var ids = uidoc.Selection.GetElementIds();

            foreach (var id in ids)
            {
                var elm = doc.GetElement(id);
                var box = elm.get_BoundingBox(view);
                if (box.Max.X > Max_X) Max_X = box.Max.X;
                if (box.Max.Y > Max_Y) Max_Y = box.Max.Y;
                if (box.Max.Z > Max_Z) Max_Z = box.Max.Z;

                if (box.Min.X < Min_X) Min_X = box.Min.X;
                if (box.Min.Y < Min_Y) Min_Y = box.Min.Y;
                if (box.Min.Z < Min_Z) Min_Z = box.Min.Z;
            }

            var Max = new XYZ(Max_X, Max_Y, Max_Z);
            var Min = new XYZ(Min_X, Min_Y, Min_Z);

            var myBox = new BoundingBoxXYZ();

            myBox.Min = Min;
            myBox.Max = Max;

            (view as View3D).SetSectionBox(myBox);
        }

        internal static bool IsElementOutsideCropBox(Element e, View v)
        {
            var rc = v.CropBoxActive;

            if (rc)
            {
                var vBox = v.CropBox;
                var eBox = e.get_BoundingBox(v);

                var tInv = v.CropBox.Transform.Inverse;
                eBox.Max = tInv.OfPoint(eBox.Max);
                eBox.Min = tInv.OfPoint(eBox.Min);

                rc = eBox.Min.X > vBox.Max.X
                     || eBox.Max.X < vBox.Min.X
                     || eBox.Min.Y > vBox.Max.Y
                     || eBox.Max.Y < vBox.Min.Y;
            }

            return rc;
        }

        internal static void AdjustViewCropToSectionBox(View3D view)
        {
            if (!view.IsSectionBoxActive) return;
            if (!view.CropBoxActive) view.CropBoxActive = true;
            var CropBox = view.CropBox;
            var SectionBox = view.GetSectionBox();
            var T = CropBox.Transform;
            var Corners = CropToRoomBBCorners(SectionBox, T);
            var MinX = Corners.Min(j => j.X);
            var MinY = Corners.Min(j => j.Y);
            var MinZ = Corners.Min(j => j.Z);
            var MaxX = Corners.Max(j => j.X);
            var MaxY = Corners.Max(j => j.Y);
            var MaxZ = Corners.Max(j => j.Z);
            CropBox.Min = new XYZ(MinX, MinY, MinZ);
            CropBox.Max = new XYZ(MaxX, MaxY, MaxZ);
            view.CropBox = CropBox;
        }

        private static XYZ[] CropToRoomBBCorners(BoundingBoxXYZ SectionBox, Transform T)
        {
            var sbmn = SectionBox.Min;
            var sbmx = SectionBox.Max;
            var Btm_LL = sbmn;
            var Btm_LR = new XYZ(sbmx.X, sbmn.Y, sbmn.Z);
            var Btm_UL = new XYZ(sbmn.X, sbmx.Y, sbmn.Z);
            var Btm_UR = new XYZ(sbmx.X, sbmx.Y, sbmn.Z);
            var Top_UR = sbmx;
            var Top_UL = new XYZ(sbmn.X, sbmx.Y, sbmx.Z);
            var Top_LR = new XYZ(sbmx.X, sbmn.Y, sbmx.Z);
            var Top_LL = new XYZ(sbmn.X, sbmn.Y, sbmx.Z);
            var Out = new XYZ[8]
            {
                Btm_LL, Btm_LR, Btm_UL, Btm_UR,
                Top_UR, Top_UL, Top_LR, Top_LL
            };
            for (int i = 0, loopTo = Out.Length - 1; i <= loopTo; i++)
            {
                Out[i] = SectionBox.Transform.OfPoint(Out[i]);
                Out[i] = T.Inverse.OfPoint(Out[i]);
            }

            return Out;
        }
    }
}
